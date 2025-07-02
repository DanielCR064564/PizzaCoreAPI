using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaCoreAPI.Data;
using PizzaCoreAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaCoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoDetallesController : ControllerBase
    {
        private readonly PizzaDbContext _context;

        public PedidoDetallesController(PizzaDbContext context)
        {
            _context = context;
        }

        // GET: api/PedidoDetalles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoDetalle>>> GetPedidoDetalles()
        {
            return await _context.PedidoDetalles
                .Include(d => d.Pedido)
                .Include(d => d.Producto)
                .ToListAsync();
        }

        // GET: api/PedidoDetalles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoDetalle>> GetPedidoDetalle(int id)
        {
            var detalle = await _context.PedidoDetalles
                .Include(pd => pd.Producto)
                .Include(d => d.Pedido)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (detalle == null)
            {
                return NotFound();
            }

            return detalle;
        }

        // POST: api/PedidoDetalles
        [HttpPost]
        public async Task<ActionResult<PedidoDetalle>> PostPedidoDetalle(PedidoDetalle detalle)
        {
            _context.PedidoDetalles.Add(detalle);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPedidoDetalle", new { id = detalle.Id }, detalle);
        }

        // PUT: api/PedidoDetalles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedidoDetalle(int id, PedidoDetalle detalle)
        {
            if (id != detalle.Id)
            {
                return BadRequest();
            }

            _context.Entry(detalle).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/PedidoDetalles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedidoDetalle(int id)
        {
            var detalle = await _context.PedidoDetalles.FindAsync(id);
            if (detalle == null)
            {
                return NotFound();
            }

            _context.PedidoDetalles.Remove(detalle);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
