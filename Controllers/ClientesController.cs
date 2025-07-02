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
    public class ClientesController : ControllerBase
    {
        private readonly PizzaDbContext _context;

        public ClientesController(PizzaDbContext context)
        {
            _context = context;
        }

        // GET: api/Clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetClientes()
        {
            var clientes = await _context.Usuarios
                .Where(u => !u.EsEmpleado)
                .ToListAsync();
            return clientes;
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetCliente(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Pedidos)
                .Where(u => !u.EsEmpleado)
                .FirstOrDefaultAsync(u => u.Id == id.ToString());

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // POST: api/Clientes
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostCliente(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCliente", new { id = usuario.Id }, usuario);
        }

        // PUT: api/Clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, Usuario usuario)
        {
            if (id.ToString() != usuario.Id)
            {
                return BadRequest(new { message = "El ID no coincide con el usuario" });
            }

            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var usuario = await _context.Usuarios
                .Where(u => !u.EsEmpleado && u.Id == id.ToString())
                .FirstOrDefaultAsync();
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
