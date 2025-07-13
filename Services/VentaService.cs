using Microsoft.EntityFrameworkCore;
using PizzaCoreAPI.Data;
using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Models;
using PizzaCoreAPI.Interfaces;

namespace PizzaCoreAPI.Services
{
    public class VentaService : IVentaService
    {
        private readonly PizzaDbContext _context;
        private readonly IPedidoService _pedidoService;
        private readonly IFacturaService _facturaService;

        public VentaService(PizzaDbContext context, IPedidoService pedidoService, IFacturaService facturaService)
        {
            _context = context;
            _pedidoService = pedidoService;
            _facturaService = facturaService;
        }

        public async Task<IEnumerable<VentaDTO>> GetAllVentasAsync()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();

            return ventas.Select(MapToDTO);
        }

        public async Task<VentaDTO?> GetVentaByIdAsync(Guid id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.Id == id);

            return venta != null ? MapToDTO(venta) : null;
        }

        public async Task<VentaDTO?> GetVentaByNumeroOrdenAsync(string numeroOrden)
        {
            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.NumeroOrden == numeroOrden);

            return venta != null ? MapToDTO(venta) : null;
        }

        public async Task<IEnumerable<VentaDTO>> GetVentasByClienteAsync(string clienteId)
        {
            var ventas = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .Where(v => v.ClienteId == clienteId)
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();

            return ventas.Select(MapToDTO);
        }

        public async Task<IEnumerable<VentaDTO>> GetVentasByEstadoAsync(EstadoVenta estado)
        {
            var ventas = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .Where(v => v.Estado == estado)
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();

            return ventas.Select(MapToDTO);
        }

        public async Task<IEnumerable<VentaDTO>> GetVentasByFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var ventas = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .Where(v => v.FechaVenta >= fechaInicio && v.FechaVenta <= fechaFin)
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();

            return ventas.Select(MapToDTO);
        }

        public async Task<VentaDTO> CreateVentaAsync(CrearVentaDTO dto, bool validateClients = true)
        {
            // Validate that details are provided
            if (dto.Detalles == null || !dto.Detalles.Any())
                throw new InvalidOperationException("La venta debe incluir al menos un producto");

            // Validate product availability
            foreach (var detalle in dto.Detalles)
            {
                var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                if (producto == null || !producto.Disponible)
                    throw new InvalidOperationException($"Producto {detalle.ProductoId} no disponible");
            }

            // Validate customer exists if provided (skip validation for sync operations)
            if (validateClients && !string.IsNullOrWhiteSpace(dto.ClienteId))
            {
                var clienteExists = await _context.Usuarios.AnyAsync(u => u.Id == dto.ClienteId && !u.EsEmpleado);
                if (!clienteExists)
                    throw new InvalidOperationException("Cliente no encontrado");
            }

            // Validate employee exists if provided (skip validation for sync operations)
            if (validateClients && !string.IsNullOrWhiteSpace(dto.EmpleadoId))
            {
                var empleadoExists = await _context.Usuarios.AnyAsync(u => u.Id == dto.EmpleadoId && u.EsEmpleado);
                if (!empleadoExists)
                    throw new InvalidOperationException("Empleado no encontrado");
            }

            // Use MySQL execution strategy for transactions
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Create sale
                    var venta = new Venta
                    {
                        Id = Guid.NewGuid(),
                        NumeroOrden = dto.NumeroOrden,
                        ClienteId = validateClients ? dto.ClienteId : null,
                        EmpleadoId = validateClients ? dto.EmpleadoId : null,
                        FechaVenta = dto.FechaVenta,
                        MetodoPago = Enum.Parse<MetodoPago>(dto.MetodoPago, ignoreCase: true),
                        TerminalId = dto.TerminalId,
                        Estado = EstadoVenta.Completada,
                        Total = 0
                    };

                    _context.Ventas.Add(venta);
                    await _context.SaveChangesAsync();

                    // Create sale details and calculate total
                    decimal total = 0;
                    foreach (var detalleDto in dto.Detalles)
                    {
                        var producto = await _context.Productos.FindAsync(detalleDto.ProductoId);
                        if (producto == null)
                            throw new InvalidOperationException($"Producto {detalleDto.ProductoId} no encontrado");

                        var detalle = new VentaDetalle
                        {
                            Id = Guid.NewGuid(),
                            VentaId = venta.Id,
                            ProductoId = detalleDto.ProductoId,
                            Cantidad = detalleDto.Cantidad,
                            PrecioUnitario = detalleDto.PrecioUnitario > 0 ? detalleDto.PrecioUnitario : producto.Precio,
                            Subtotal = (detalleDto.PrecioUnitario > 0 ? detalleDto.PrecioUnitario : producto.Precio) * detalleDto.Cantidad
                        };

                        total += detalle.Subtotal;
                        _context.VentaDetalles.Add(detalle);
                    }

                    venta.Total = total;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return await GetVentaByIdAsync(venta.Id) ??
                           throw new InvalidOperationException("Error retrieving created sale");
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<VentaDTO> CreateVentaFromSyncAsync(SyncVentaDTO syncDto)
        {
            // Check if sale already exists
            var existingVenta = await _context.Ventas
                .FirstOrDefaultAsync(v => v.NumeroOrden == syncDto.NumeroOrden);

            if (existingVenta != null)
                return MapToDTO(existingVenta);

            // Create the sale without nested transactions
            var createVentaDto = new CrearVentaDTO
            {
                NumeroOrden = syncDto.NumeroOrden,
                ClienteId = syncDto.ClienteId ?? "cliente-pos",
                FechaVenta = syncDto.FechaVenta,
                Total = syncDto.Total,
                MetodoPago = syncDto.MetodoPago,
                TerminalId = syncDto.TerminalId,
                Detalles = syncDto.Detalles.Select(d => new CrearVentaDetalleDTO
                {
                    ProductoId = Guid.Parse(d.ProductoApiId),
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList()
            };

            var venta = await CreateVentaAsync(createVentaDto, false);

            // Also create a Pedido for compatibility (without transaction)
            try
            {
                var pedidoDto = new CrearPedidoDTO
                {
                    ClienteId = syncDto.ClienteId ?? "cliente-pos",
                    FechaPedido = syncDto.FechaVenta,
                    Estado = EstadoPedido.Entregado.ToString(),
                    MetodoPago = syncDto.MetodoPago,
                    Total = syncDto.Total,
                    NumeroOrden = syncDto.NumeroOrden,
                    TerminalId = syncDto.TerminalId,
                    Detalles = syncDto.Detalles.Select(d => new CrearPedidoDetalleDTO
                    {
                        ProductoId = Guid.Parse(d.ProductoApiId),
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Cantidad * d.PrecioUnitario
                    }).ToList()
                };

                var pedido = await _pedidoService.CreatePedidoAsync(pedidoDto);

                // Link the venta to the pedido
                var ventaEntity = await _context.Ventas.FindAsync(venta.Id);
                if (ventaEntity != null)
                {
                    ventaEntity.PedidoId = pedido.Id;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the sale creation
                // The sale was created successfully, just couldn't link to pedido
                Console.WriteLine($"Warning: Could not create linked pedido: {ex.Message}");
            }

            return venta;
        }

        private async Task<VentaDTO> CreateVentaFromDtoInTransactionAsync(CrearVentaDTO dto)
        {
            var venta = new Venta
            {
                Id = Guid.NewGuid(),
                NumeroOrden = dto.NumeroOrden,
                ClienteId = dto.ClienteId,
                EmpleadoId = dto.EmpleadoId,
                FechaVenta = dto.FechaVenta,
                MetodoPago = Enum.Parse<MetodoPago>(dto.MetodoPago, ignoreCase: true),
                TerminalId = dto.TerminalId,
                Estado = EstadoVenta.Completada,
                Total = 0
            };

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            // Create sale details
            decimal total = 0;
            foreach (var detalleDto in dto.Detalles)
            {
                var producto = await _context.Productos.FindAsync(detalleDto.ProductoId);
                if (producto == null)
                    throw new InvalidOperationException($"Producto {detalleDto.ProductoId} no encontrado");

                var detalle = new VentaDetalle
                {
                    Id = Guid.NewGuid(),
                    VentaId = venta.Id,
                    ProductoId = detalleDto.ProductoId,
                    Cantidad = detalleDto.Cantidad,
                    PrecioUnitario = detalleDto.PrecioUnitario > 0 ? detalleDto.PrecioUnitario : producto.Precio,
                    Subtotal = (detalleDto.PrecioUnitario > 0 ? detalleDto.PrecioUnitario : producto.Precio) * detalleDto.Cantidad
                };

                total += detalle.Subtotal;
                _context.VentaDetalles.Add(detalle);
            }

            venta.Total = total;
            await _context.SaveChangesAsync();

            return MapToDTO(venta);
        }

        public async Task<bool> UpdateVentaAsync(Guid id, CrearVentaDTO dto)
        {
            var venta = await _context.Ventas
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null) return false;

            // Business rule: Cannot update completed sales
            if (venta.Estado == EstadoVenta.Completada)
                throw new InvalidOperationException("No se puede modificar una venta completada");

            // Use MySQL execution strategy for transactions
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Update basic properties
                    venta.EmpleadoId = dto.EmpleadoId;
                    venta.MetodoPago = Enum.Parse<MetodoPago>(dto.MetodoPago, ignoreCase: true);
                    venta.TerminalId = dto.TerminalId;
                    venta.FechaActualizacion = DateTime.Now;

                    // Update details if provided
                    if (dto.Detalles?.Any() == true)
                    {
                        // Remove existing details
                        _context.VentaDetalles.RemoveRange(venta.Detalles);

                        // Add new details
                        decimal total = 0;
                        foreach (var detalleDto in dto.Detalles)
                        {
                            var producto = await _context.Productos.FindAsync(detalleDto.ProductoId);
                            if (producto == null || !producto.Disponible)
                                throw new InvalidOperationException($"Producto {detalleDto.ProductoId} no disponible");

                            var detalle = new VentaDetalle
                            {
                                Id = Guid.NewGuid(),
                                VentaId = venta.Id,
                                ProductoId = detalleDto.ProductoId,
                                Cantidad = detalleDto.Cantidad,
                                PrecioUnitario = detalleDto.PrecioUnitario > 0 ? detalleDto.PrecioUnitario : producto.Precio,
                                Subtotal = (detalleDto.PrecioUnitario > 0 ? detalleDto.PrecioUnitario : producto.Precio) * detalleDto.Cantidad
                            };

                            total += detalle.Subtotal;
                            _context.VentaDetalles.Add(detalle);
                        }

                        venta.Total = total;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<bool> DeleteVentaAsync(Guid id)
        {
            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null) return false;

            // Business rule: Cannot delete completed sales, only cancel them
            if (venta.Estado == EstadoVenta.Completada)
                throw new InvalidOperationException("No se puede eliminar una venta completada. Use cancelar en su lugar.");

            _context.Ventas.Remove(venta);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelarVentaAsync(Guid id)
        {
            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null) return false;

            if (venta.Estado == EstadoVenta.Cancelada)
                throw new InvalidOperationException("La venta ya está cancelada");

            venta.Estado = EstadoVenta.Cancelada;
            venta.FechaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        private static VentaDTO MapToDTO(Venta venta)
        {
            return new VentaDTO
            {
                Id = venta.Id,
                NumeroOrden = venta.NumeroOrden,
                ClienteId = venta.ClienteId,
                ClienteNombre = venta.Cliente?.NombreCompleto,
                EmpleadoId = venta.EmpleadoId,
                EmpleadoNombre = venta.Empleado?.NombreCompleto,
                FechaVenta = venta.FechaVenta,
                Total = venta.Total,
                MetodoPago = venta.MetodoPago.ToString(),
                TerminalId = venta.TerminalId,
                Estado = venta.Estado.ToString(),
                PedidoId = venta.PedidoId,
                FacturaId = venta.FacturaId,
                FechaCreacion = venta.FechaCreacion,
                Detalles = venta.Detalles?.Select(d => new VentaDetalleDTO
                {
                    Id = d.Id,
                    ProductoId = d.ProductoId,
                    ProductoNombre = d.Producto?.Nombre ?? "",
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList() ?? new List<VentaDetalleDTO>()
            };
        }
    }
}
