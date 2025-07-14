using Microsoft.EntityFrameworkCore;
using PizzaCoreAPI.Data;
using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Models;
using PizzaCoreAPI.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace PizzaCoreAPI.Services
{
    public class VentaService : IVentaService
    {
        private readonly PizzaDbContext _context;
        private readonly IPedidoService _pedidoService;
        private readonly IFacturaService _facturaService;
        private readonly ILogger<VentaService> _logger;

        public VentaService(PizzaDbContext context, IPedidoService pedidoService, IFacturaService facturaService, ILogger<VentaService> logger)
        {
            _context = context;
            _pedidoService = pedidoService;
            _facturaService = facturaService;
            _logger = logger;
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

        public async Task<VentaDTO?> GetVentaByIdAsync(string id)
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
                        Id = Guid.NewGuid().ToString(),
                        NumeroOrden = dto.NumeroOrden,
                        ClienteId = validateClients ? dto.ClienteId : null,
                        EmpleadoId = validateClients ? dto.EmpleadoId : null,
                        FechaVenta = dto.FechaVenta,
                        MetodoPago = ParseMetodoPago(dto.MetodoPago),
                        TerminalId = dto.TerminalId,
                        Estado = EstadoVenta.Completada,
                        Total = 0,
                        PedidoId = null,
                        FacturaId = null
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
                            Id = Guid.NewGuid().ToString(),
                            VentaId = venta.Id,
                            ProductoId = detalleDto.ProductoId, // Keep as Guid - matches Productos table
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

            var createVentaDto = new CrearVentaDTO
            {
                NumeroOrden = syncDto.NumeroOrden,
                ClienteId = null, // Don't set for sync to avoid foreign key issues
                EmpleadoId = null, // Don't set for sync to avoid foreign key issues
                FechaVenta = syncDto.FechaVenta,
                Total = syncDto.Total,
                MetodoPago = syncDto.MetodoPago,
                TerminalId = null, // Set to null to avoid issues
                Detalles = syncDto.Detalles
                    .Where(d => !string.IsNullOrEmpty(d.ProductoApiId) && Guid.TryParse(d.ProductoApiId, out _))
                    .Select(d => new CrearVentaDetalleDTO
                    {
                        ProductoId = Guid.Parse(d.ProductoApiId), // Convert string to Guid for ProductoId
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario
                    }).ToList()
            };

            if (createVentaDto.Detalles == null || !createVentaDto.Detalles.Any())
            {
                throw new InvalidOperationException("All Detalles have invalid or missing ProductoApiId.");
            }

            var venta = await CreateVentaAsync(createVentaDto, false);
            return venta;
        }

        public async Task<bool> UpdateVentaAsync(string id, CrearVentaDTO dto)
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
                    venta.MetodoPago = ParseMetodoPago(dto.MetodoPago);
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
                                Id = Guid.NewGuid().ToString(),
                                VentaId = venta.Id,
                                ProductoId = detalleDto.ProductoId, // Keep as Guid
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

        public async Task<bool> DeleteVentaAsync(string id)
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

        public async Task<bool> CancelarVentaAsync(string id)
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
                Id = Guid.Parse(venta.Id), // Convert string ID back to Guid for DTO compatibility
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
                FacturaId = string.IsNullOrEmpty(venta.FacturaId?.ToString()) ? null : Guid.Parse(venta.FacturaId!.ToString()),
                FechaCreacion = venta.FechaCreacion,
                Detalles = venta.Detalles?.Select(d => new VentaDetalleDTO
                {
                    Id = Guid.Parse(d.Id), // Convert string ID back to Guid for DTO compatibility
                    ProductoId = d.ProductoId, // ProductoId remains Guid
                    ProductoNombre = d.Producto?.Nombre ?? "",
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList() ?? new List<VentaDetalleDTO>()
            };
        }

        private static MetodoPago ParseMetodoPago(string metodoPagoStr)
        {
            if (string.IsNullOrWhiteSpace(metodoPagoStr))
                return MetodoPago.Efectivo;

            // If it's a number, parse it as enum value
            if (int.TryParse(metodoPagoStr, out var metodoPagoInt))
            {
                if (Enum.IsDefined(typeof(MetodoPago), metodoPagoInt))
                    return (MetodoPago)metodoPagoInt;
                return MetodoPago.Efectivo;
            }

            // If it's a string, try to parse it as enum name or convert common strings
            return metodoPagoStr.ToLower().Trim() switch
            {
                "efectivo" => MetodoPago.Efectivo,
                "tarjeta" => MetodoPago.Tarjeta,
                "transferencia" => MetodoPago.Transferencia,
                _ => Enum.TryParse<MetodoPago>(metodoPagoStr, ignoreCase: true, out var parsed) ? parsed : MetodoPago.Efectivo
            };
        }
    }
}