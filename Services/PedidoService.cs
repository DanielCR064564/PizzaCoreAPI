using Microsoft.EntityFrameworkCore;
using PizzaCoreAPI.Data;
using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Models;
using PizzaCoreAPI.Interfaces;

namespace PizzaCoreAPI.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly PizzaDbContext _context;

        public PedidoService(PizzaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PedidoDTO>> GetAllPedidosAsync()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Empleado)
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();

            return pedidos.Select(MapToDTO);
        }

        public async Task<PedidoDTO?> GetPedidoByIdAsync(string id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Empleado)
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);

            return pedido != null ? MapToDTO(pedido) : null;
        }

        public async Task<IEnumerable<PedidoDTO>> GetPedidosByClienteAsync(string clienteId)
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Empleado)
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .Where(p => p.ClienteId == clienteId)
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();

            return pedidos.Select(MapToDTO);
        }

        public async Task<IEnumerable<PedidoDTO>> GetPedidosByEstadoAsync(EstadoPedido estado)
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Empleado)
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .Where(p => p.Estado == estado)
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();

            return pedidos.Select(MapToDTO);
        }

        public async Task<PedidoDTO> CreatePedidoAsync(CrearPedidoDTO dto)
        {
            // Validate that details are provided
            if (dto.Detalles == null || !dto.Detalles.Any())
                throw new InvalidOperationException("El pedido debe incluir al menos un producto");

            // Validate product availability
            if (!await ValidarDisponibilidadProductosAsync(dto.Detalles))
                throw new InvalidOperationException("Uno o más productos no están disponibles");

            // Validate customer exists
            var clienteExists = await _context.Usuarios.AnyAsync(u => u.Id == dto.ClienteId && !u.EsEmpleado);
            if (!clienteExists)
                throw new InvalidOperationException("Cliente no encontrado");

            // Validate employee if provided
            if (!string.IsNullOrWhiteSpace(dto.EmpleadoId))
            {
                var empleadoExists = await _context.Usuarios.AnyAsync(u => u.Id == dto.EmpleadoId && u.EsEmpleado);
                if (!empleadoExists)
                    throw new InvalidOperationException("Empleado no encontrado");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Create order
                var pedido = new Pedido
                {
                    Id = Guid.NewGuid().ToString(),
                    ClienteId = dto.ClienteId,
                    EmpleadoId = string.IsNullOrWhiteSpace(dto.EmpleadoId) ? null : dto.EmpleadoId,
                    FechaPedido = DateTime.Now,
                    Estado = string.IsNullOrWhiteSpace(dto.Estado) ? EstadoPedido.Pendiente : 
                            Enum.Parse<EstadoPedido>(dto.Estado, ignoreCase: true),
                    Total = 0
                };

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                // Create order details and calculate total
                decimal total = 0;
                foreach (var detalleDto in dto.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(detalleDto.ProductoId);
                    if (producto == null || !producto.Disponible)
                        throw new InvalidOperationException($"Producto {detalleDto.ProductoId} no disponible");

                    if (detalleDto.Cantidad <= 0)
                        throw new InvalidOperationException("La cantidad debe ser mayor a 0");

                    var detalle = new PedidoDetalle
                    {
                        Id = Guid.NewGuid().ToString(),
                        PedidoId = pedido.Id,
                        ProductoId = detalleDto.ProductoId,
                        Cantidad = detalleDto.Cantidad,
                        PrecioUnitario = producto.Precio,
                        Subtotal = producto.Precio * detalleDto.Cantidad
                    };

                    total += detalle.Subtotal;
                    _context.PedidoDetalles.Add(detalle);
                }

                pedido.Total = total;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetPedidoByIdAsync(pedido.Id) ?? 
                       throw new InvalidOperationException("Error retrieving created order");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdatePedidoAsync(string id, CrearPedidoDTO dto)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null) return false;

            // Business rule: Cannot update delivered or cancelled orders
            if (pedido.Estado == EstadoPedido.Entregado || pedido.Estado == EstadoPedido.Cancelado)
                throw new InvalidOperationException("No se puede modificar un pedido entregado o cancelado");

            // Validate employee if provided
            if (!string.IsNullOrWhiteSpace(dto.EmpleadoId))
            {
                var empleadoExists = await _context.Usuarios.AnyAsync(u => u.Id == dto.EmpleadoId && u.EsEmpleado);
                if (!empleadoExists)
                    throw new InvalidOperationException("Empleado no encontrado");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update basic properties
                pedido.EmpleadoId = string.IsNullOrWhiteSpace(dto.EmpleadoId) ? null : dto.EmpleadoId;
                
                if (!string.IsNullOrWhiteSpace(dto.Estado))
                {
                    var nuevoEstado = Enum.Parse<EstadoPedido>(dto.Estado, ignoreCase: true);
                    
                    // Validate state transitions
                    if (!ValidarTransicionEstado(pedido.Estado, nuevoEstado))
                        throw new InvalidOperationException($"No se puede cambiar de {pedido.Estado} a {nuevoEstado}");
                    
                    pedido.Estado = nuevoEstado;
                    
                    // Set delivery date if delivered
                    if (nuevoEstado == EstadoPedido.Entregado)
                        pedido.FechaEntrega = DateTime.Now;
                }

                // Update details if provided
                if (dto.Detalles?.Any() == true)
                {
                    // Validate products availability
                    if (!await ValidarDisponibilidadProductosAsync(dto.Detalles))
                        throw new InvalidOperationException("Uno o más productos no están disponibles");

                    // Remove existing details
                    _context.PedidoDetalles.RemoveRange(pedido.Detalles);

                    // Add new details
                    decimal total = 0;
                    foreach (var detalleDto in dto.Detalles)
                    {
                        var producto = await _context.Productos.FindAsync(detalleDto.ProductoId);
                        if (producto == null || !producto.Disponible)
                            throw new InvalidOperationException($"Producto {detalleDto.ProductoId} no disponible");

                        if (detalleDto.Cantidad <= 0)
                            throw new InvalidOperationException("La cantidad debe ser mayor a 0");

                        var detalle = new PedidoDetalle
                        {
                            Id = Guid.NewGuid().ToString(),
                            PedidoId = pedido.Id,
                            ProductoId = detalleDto.ProductoId,
                            Cantidad = detalleDto.Cantidad,
                            PrecioUnitario = producto.Precio,
                            Subtotal = producto.Precio * detalleDto.Cantidad
                        };

                        total += detalle.Subtotal;
                        _context.PedidoDetalles.Add(detalle);
                    }

                    pedido.Total = total;
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
        }

        public async Task<bool> DeletePedidoAsync(string id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return false;

            // Business rule: Only pending orders can be deleted
            if (pedido.Estado != EstadoPedido.Pendiente)
                throw new InvalidOperationException("Solo se pueden eliminar pedidos pendientes");

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateEstadoPedidoAsync(string id, EstadoPedido estado)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return false;

            // Validate state transition
            if (!ValidarTransicionEstado(pedido.Estado, estado))
                throw new InvalidOperationException($"No se puede cambiar de {pedido.Estado} a {estado}");

            pedido.Estado = estado;
            
            // Set delivery date if delivered
            if (estado == EstadoPedido.Entregado)
                pedido.FechaEntrega = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> CalcularTotalPedidoAsync(List<CrearPedidoDetalleDTO> detalles)
        {
            decimal total = 0;
            foreach (var detalle in detalles)
            {
                var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                if (producto != null)
                {
                    total += producto.Precio * detalle.Cantidad;
                }
            }
            return total;
        }

        public async Task<bool> ValidarDisponibilidadProductosAsync(List<CrearPedidoDetalleDTO> detalles)
        {
            foreach (var detalle in detalles)
            {
                var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                if (producto == null || !producto.Disponible)
                    return false;
                
                if (detalle.Cantidad <= 0)
                    return false;
            }
            return true;
        }

        private static bool ValidarTransicionEstado(EstadoPedido estadoActual, EstadoPedido nuevoEstado)
        {
            // Define valid state transitions
            return estadoActual switch
            {
                EstadoPedido.Pendiente => nuevoEstado is EstadoPedido.EnPreparacion or EstadoPedido.Cancelado,
                EstadoPedido.EnPreparacion => nuevoEstado is EstadoPedido.Listo or EstadoPedido.Cancelado,
                EstadoPedido.Listo => nuevoEstado is EstadoPedido.EnCamino or EstadoPedido.Entregado,
                EstadoPedido.EnCamino => nuevoEstado is EstadoPedido.Entregado,
                EstadoPedido.Entregado => false, // Cannot change from delivered
                EstadoPedido.Cancelado => false, // Cannot change from cancelled
                _ => false
            };
        }

        private static PedidoDTO MapToDTO(Pedido pedido)
        {
            return new PedidoDTO
            {
                Id = pedido.Id,
                FechaPedido = pedido.FechaPedido,
                Estado = pedido.Estado.ToString(),
                ClienteId = pedido.ClienteId,
                EmpleadoId = pedido.EmpleadoId,
                Total = pedido.Total,
                Detalles = pedido.Detalles?.Select(d => new PedidoDetalleDTO
                {
                    Id = d.Id,
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal,
                    Producto = new ProductoDTO
                    {
                        Id = d.Producto.Id,
                        Nombre = d.Producto.Nombre,
                        Precio = d.Producto.Precio,
                        Tipo = d.Producto.Tipo.ToString(),
                        Disponible = d.Producto.Disponible
                    }
                }).ToList() ?? new List<PedidoDetalleDTO>()
            };
        }
    }
}