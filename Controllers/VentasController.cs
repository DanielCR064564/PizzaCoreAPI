using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Models;
using PizzaCoreAPI.Interfaces;

namespace PizzaCoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Empleado,Administrador")]
    public class VentasController : ControllerBase
    {
        private readonly IVentaService _ventaService;

        public VentasController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        /// <summary>
        /// Gets all sales
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VentaDTO>>> GetVentas()
        {
            try
            {
                var ventas = await _ventaService.GetAllVentasAsync();
                return Ok(ventas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving sales", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets a specific sale by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<VentaDTO>> GetVenta(string id)
        {
            try
            {
                var venta = await _ventaService.GetVentaByIdAsync(id);
                if (venta == null) 
                    return NotFound(new { message = "Venta no encontrada" });
                
                return Ok(venta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving sale", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets sale by order number
        /// </summary>
        [HttpGet("numero/{numeroOrden}")]
        public async Task<ActionResult<VentaDTO>> GetVentaByNumeroOrden(string numeroOrden)
        {
            try
            {
                var venta = await _ventaService.GetVentaByNumeroOrdenAsync(numeroOrden);
                if (venta == null) 
                    return NotFound(new { message = "No se encontró venta con ese número de orden" });
                
                return Ok(venta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving sale by order number", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets sales by customer
        /// </summary>
        [HttpGet("cliente/{clienteId}")]
        public async Task<ActionResult<IEnumerable<VentaDTO>>> GetVentasByCliente(string clienteId)
        {
            try
            {
                var ventas = await _ventaService.GetVentasByClienteAsync(clienteId);
                return Ok(ventas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving sales by customer", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets sales by status
        /// </summary>
        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<IEnumerable<VentaDTO>>> GetVentasByEstado(EstadoVenta estado)
        {
            try
            {
                var ventas = await _ventaService.GetVentasByEstadoAsync(estado);
                return Ok(ventas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving sales by status", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets sales by date range
        /// </summary>
        [HttpGet("fecha")]
        public async Task<ActionResult<IEnumerable<VentaDTO>>> GetVentasByFecha(
            [FromQuery] DateTime fechaInicio, 
            [FromQuery] DateTime fechaFin)
        {
            try
            {
                var ventas = await _ventaService.GetVentasByFechaAsync(fechaInicio, fechaFin);
                return Ok(ventas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving sales by date", error = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new sale
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<VentaDTO>> CreateVenta([FromBody] CrearVentaDTO dto)
        {
            try
            {
                var venta = await _ventaService.CreateVentaAsync(dto);
                return CreatedAtAction(nameof(GetVenta), new { id = venta.Id.ToString() }, venta);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating sale", error = ex.Message });
            }
        }

        /// <summary>
        /// Updates a sale
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVenta(string id, [FromBody] CrearVentaDTO dto)
        {
            try
            {
                var success = await _ventaService.UpdateVentaAsync(id, dto);
                if (!success)
                    return NotFound(new { message = "Venta no encontrada" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating sale", error = ex.Message });
            }
        }

        /// <summary>
        /// Cancels a sale
        /// </summary>
        [HttpPut("{id}/cancelar")]
        public async Task<IActionResult> CancelarVenta(string id)
        {
            try
            {
                var success = await _ventaService.CancelarVentaAsync(id);
                if (!success)
                    return NotFound(new { message = "Venta no encontrada" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error cancelling sale", error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a sale
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenta(string id)
        {
            try
            {
                var success = await _ventaService.DeleteVentaAsync(id);
                if (!success)
                    return NotFound(new { message = "Venta no encontrada" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting sale", error = ex.Message });
            }
        }
    }
}