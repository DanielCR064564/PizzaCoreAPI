using Microsoft.AspNetCore.Mvc;
using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaCoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        // GET: api/Clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDTO>>> GetClientes()
        {
            var clientes = await _clienteService.GetAllClientesAsync();
            return Ok(clientes);
        }

        // GET: api/Clientes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDTO>> GetCliente(string id)
        {
            var cliente = await _clienteService.GetClienteByIdAsync(id);
            
            if (cliente == null)
            {
                return NotFound(new { message = "Cliente no encontrado" });
            }

            return Ok(cliente);
        }

        // POST: api/Clientes
        [HttpPost]
        public async Task<ActionResult<ClienteDTO>> PostCliente([FromBody] CrearClienteDTO clienteDto)
        {
            try
            {
                // ✅ Validate unique constraints
                if (await _clienteService.UserNameExistsAsync(clienteDto.UserName))
                {
                    return BadRequest(new { message = "El nombre de usuario ya existe" });
                }

                if (await _clienteService.EmailExistsAsync(clienteDto.Email))
                {
                    return BadRequest(new { message = "El email ya está registrado" });
                }

                var cliente = await _clienteService.CreateClienteAsync(clienteDto);
                return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, cliente);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Clientes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(string id, [FromBody] ActualizarClienteDTO clienteDto)
        {
            try
            {
                // ✅ Check if client exists
                if (!await _clienteService.ClienteExistsAsync(id))
                {
                    return NotFound(new { message = "Cliente no encontrado" });
                }

                var success = await _clienteService.UpdateClienteAsync(id, clienteDto);
                
                if (!success)
                {
                    return BadRequest(new { message = "Error al actualizar el cliente" });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Clientes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(string id)
        {
            var success = await _clienteService.DeleteClienteAsync(id);
            
            if (!success)
            {
                return NotFound(new { message = "Cliente no encontrado" });
            }

            return NoContent();
        }

        // GET: api/Clientes/validate/username/{username}
        [HttpGet("validate/username/{username}")]
        public async Task<ActionResult<bool>> ValidateUsername(string username)
        {
            var exists = await _clienteService.UserNameExistsAsync(username);
            return Ok(new { exists, available = !exists });
        }

        // GET: api/Clientes/validate/email/{email}
        [HttpGet("validate/email/{email}")]
        public async Task<ActionResult<bool>> ValidateEmail(string email)
        {
            var exists = await _clienteService.EmailExistsAsync(email);
            return Ok(new { exists, available = !exists });
        }
    }
}