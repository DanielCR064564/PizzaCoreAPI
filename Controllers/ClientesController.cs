using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaCoreAPI.Data;
using PizzaCoreAPI.Models;
using System.Collections.Generic;
using System.Linq;
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

        // GET: api/Clientes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetCliente(string id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Pedidos)
                .Where(u => !u.EsEmpleado)
                .FirstOrDefaultAsync(u => u.Id == id);

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
            usuario.EsEmpleado = false; // Forzar que sea cliente
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCliente), new { id = usuario.Id }, usuario);
        }

        // PUT: api/Clientes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(string id, Usuario usuario)
        {
            // Forzar que el ID recibido en el body coincida con el de la URL
            usuario.Id = id;

            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id && !u.EsEmpleado);

            if (usuarioExistente == null)
            {
                return NotFound();
            }

            // Actualizar solo campos relevantes
            usuarioExistente.NombreCompleto = usuario.NombreCompleto;
            usuarioExistente.UserName = usuario.UserName;
            usuarioExistente.Email = usuario.Email;
            usuarioExistente.PhoneNumber = usuario.PhoneNumber;
            usuarioExistente.Direccion = usuario.Direccion;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cliente actualizado correctamente" });
        }

        // DELETE: api/Clientes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(string id)
        {
            var usuario = await _context.Usuarios
                .Where(u => !u.EsEmpleado && u.Id == id)
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
