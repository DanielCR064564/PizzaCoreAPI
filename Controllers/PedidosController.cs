using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaCoreAPI.Data;
using PizzaCoreAPI.Models;
using PizzaCoreAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace PizzaCoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Empleado,Administrador")]
    public class PedidosController : ControllerBase
    {
        private readonly PizzaDbContext _context;

        public PedidosController(PizzaDbContext context)
        {
            _context = context;
        }

        // GET: api/Pedidos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoDTO>>> GetPedidos()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Empleado)
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .ToListAsync();

            return Ok(pedidos.Select(p => new PedidoDTO
            {
                Id = p.Id.ToString(),
                FechaPedido = p.FechaPedido,
                Estado = p.Estado,
                ClienteId = p.ClienteId.ToString(),
                EmpleadoId = p.EmpleadoId.ToString(),
                Total = p.Total,
                Detalles = p.Detalles.Select(d => new PedidoDetalleDTO
                {
                    Id = d.Id.ToString(),
                    ProductoId = d.ProductoId.ToString(),
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal,
                    Producto = new ProductoDTO
                    {
                        Id = d.Producto.Id,
                        Nombre = d.Producto.Nombre,
                        Precio = d.Producto.Precio,
                        Tipo = d.Producto.Tipo.ToString()
                    }
                }).ToList()
            }));
        }

        // GET: api/Pedidos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(string id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Empleado)
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }

            return Ok(pedido);
        }

        // POST: api/Pedidos
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Pedido>> PostPedido(CrearPedidoDTO pedidoDto)
        {
            var pedido = new Pedido
            {
                Id = Guid.NewGuid().ToString(),
                FechaPedido = DateTime.Now,
                Estado = pedidoDto.Estado,
                ClienteId = pedidoDto.ClienteId,
                EmpleadoId = pedidoDto.EmpleadoId,
                Total = 0
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            decimal total = 0;
            foreach (var detalle in pedidoDto.Detalles)
            {
                var producto = await _context.Productos.FindAsync(Guid.Parse(detalle.ProductoId));
                if (producto == null || !producto.Disponible)
                    throw new InvalidOperationException("Producto no disponible");

                var detallePedido = new PedidoDetalle
                {
                    Id = Guid.NewGuid().ToString(),
                    PedidoId = pedido.Id,
                    ProductoId = Guid.Parse(detalle.ProductoId),
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = producto.Precio,
                    Subtotal = producto.Precio * decimal.Parse(detalle.Cantidad)
                };

                total += detallePedido.Subtotal;
                _context.PedidoDetalles.Add(detallePedido);
            }

            pedido.Total = total;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPedido", new { id = pedido.Id }, pedido);
        }

        // PUT: api/Pedidos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedido(string id, CrearPedidoDTO pedidoDto)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }

            if (pedido.Estado == "Cancelado" && pedidoDto.Estado == "Entregado")
            {
                return BadRequest("No se puede entregar un pedido cancelado.");
            }

            pedido.EmpleadoId = pedidoDto.EmpleadoId;
            pedido.Estado = pedidoDto.Estado;

            if (pedidoDto.Estado == "Entregado" && pedido.FechaEntrega == null)
            {
                pedido.FechaEntrega = DateTime.Now;
            }

            _context.PedidoDetalles.RemoveRange(pedido.Detalles);
            await _context.SaveChangesAsync();

            decimal total = 0;
            foreach (var detalle in pedidoDto.Detalles)
            {
                var producto = await _context.Productos.FindAsync(Guid.Parse(detalle.ProductoId));
                if (producto == null || !producto.Disponible)
                    return BadRequest("Producto no disponible");

                var detallePedido = new PedidoDetalle
                {
                    Id = Guid.NewGuid().ToString(),
                    PedidoId = pedido.Id,
                    ProductoId = Guid.Parse(detalle.ProductoId),
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = producto.Precio,
                    Subtotal = producto.Precio * decimal.Parse(detalle.Cantidad)
                };

                total += detallePedido.Subtotal;
                _context.PedidoDetalles.Add(detallePedido);
            }

            pedido.Total = total;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Pedidos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(string id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
