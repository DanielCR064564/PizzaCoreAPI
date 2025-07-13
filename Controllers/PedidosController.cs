using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Models;
using PizzaCoreAPI.Services;
using PizzaCoreAPI.Interfaces;

namespace PizzaCoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Empleado,Administrador")]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidosController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        /// <summary>
        /// Gets all orders with their details
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoDTO>>> GetPedidos()
        {
            try
            {
                var pedidos = await _pedidoService.GetAllPedidosAsync();
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving orders", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets a specific order by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoDTO>> GetPedido(string id)
        {
            try
            {
                var pedido = await _pedidoService.GetPedidoByIdAsync(id);
                if (pedido == null) 
                    return NotFound(new { message = "Pedido no encontrado" });
                
                return Ok(pedido);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving order", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets orders by customer ID
        /// </summary>
        [HttpGet("cliente/{clienteId}")]
        public async Task<ActionResult<IEnumerable<PedidoDTO>>> GetPedidosByCliente(string clienteId)
        {
            try
            {
                var pedidos = await _pedidoService.GetPedidosByClienteAsync(clienteId);
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving customer orders", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets orders by status
        /// </summary>
        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<IEnumerable<PedidoDTO>>> GetPedidosByEstado(EstadoPedido estado)
        {
            try
            {
                var pedidos = await _pedidoService.GetPedidosByEstadoAsync(estado);
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving orders by status", error = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new order
        /// </summary>
        [HttpPost]
        [AllowAnonymous] // Allow customers to create orders
        public async Task<ActionResult<PedidoDTO>> CreatePedido([FromBody] CrearPedidoDTO pedidoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var pedido = await _pedidoService.CreatePedidoAsync(pedidoDto);
                return CreatedAtAction(nameof(GetPedido), new { id = pedido.Id }, pedido);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating order", error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing order
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePedido(string id, [FromBody] CrearPedidoDTO pedidoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _pedidoService.UpdatePedidoAsync(id, pedidoDto);
                if (!result) 
                    return NotFound(new { message = "Pedido no encontrado" });
                
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating order", error = ex.Message });
            }
        }

        /// <summary>
        /// Updates only the order status
        /// </summary>
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> UpdateEstadoPedido(string id, [FromBody] EstadoPedido estado)
        {
            try
            {
                var result = await _pedidoService.UpdateEstadoPedidoAsync(id, estado);
                if (!result) 
                    return NotFound(new { message = "Pedido no encontrado" });
                
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating order status", error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes an order (only if pending)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(string id)
        {
            try
            {
                var result = await _pedidoService.DeletePedidoAsync(id);
                if (!result) 
                    return NotFound(new { message = "Pedido no encontrado" });
                
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting order", error = ex.Message });
            }
        }

        /// <summary>
        /// Calculates total for order details without creating the order
        /// </summary>
        [HttpPost("calcular-total")]
        [AllowAnonymous]
        public async Task<ActionResult<decimal>> CalcularTotal([FromBody] List<CrearPedidoDetalleDTO> detalles)
        {
            try
            {
                var total = await _pedidoService.CalcularTotalPedidoAsync(detalles);
                return Ok(new { total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error calculating total", error = ex.Message });
            }
        }

        /// <summary>
        /// Validates product availability
        /// </summary>
        [HttpPost("validar-disponibilidad")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> ValidarDisponibilidad([FromBody] List<CrearPedidoDetalleDTO> detalles)
        {
            try
            {
                var disponible = await _pedidoService.ValidarDisponibilidadProductosAsync(detalles);
                return Ok(new { disponible });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error validating availability", error = ex.Message });
            }
        }
    }
}