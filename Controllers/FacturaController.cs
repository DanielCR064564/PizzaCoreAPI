using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Models;
using PizzaCoreAPI.Services;

namespace PizzaCoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Empleado,Administrador")]
    public class FacturasController : ControllerBase
    {
        private readonly IFacturaService _facturaService;

        public FacturasController(IFacturaService facturaService)
        {
            _facturaService = facturaService;
        }

        /// <summary>
        /// Gets all invoices
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacturaDTO>>> GetFacturas()
        {
            try
            {
                var facturas = await _facturaService.GetAllFacturasAsync();
                return Ok(facturas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving invoices", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets a specific invoice by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<FacturaDTO>> GetFactura(Guid id)
        {
            try
            {
                var factura = await _facturaService.GetFacturaByIdAsync(id);
                if (factura == null) 
                    return NotFound(new { message = "Factura no encontrada" });
                
                return Ok(factura);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving invoice", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets invoice by order ID
        /// </summary>
        [HttpGet("pedido/{pedidoId}")]
        public async Task<ActionResult<FacturaDTO>> GetFacturaByPedido(string pedidoId)
        {
            try
            {
                var factura = await _facturaService.GetFacturaByPedidoIdAsync(pedidoId);
                if (factura == null) 
                    return NotFound(new { message = "No se encontró factura para este pedido" });
                
                return Ok(factura);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving invoice by order", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets invoices by status
        /// </summary>
        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<IEnumerable<FacturaDTO>>> GetFacturasByEstado(EstadoFactura estado)
        {
            try
            {
                var facturas = await _facturaService.GetFacturasByEstadoAsync(estado);
                return Ok(facturas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving invoices by status", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets invoices by date range
        /// </summary>
        [HttpGet("fecha")]
        public async Task<ActionResult<IEnumerable<FacturaDTO>>> GetFacturasByFecha(
            [FromQuery] DateTime fechaInicio, 
            [FromQuery] DateTime fechaFin)
        {
            try
            {
                if (fechaInicio > fechaFin)
                    return BadRequest(new { message = "La fecha de inicio no puede ser mayor a la fecha fin" });

                var facturas = await _facturaService.GetFacturasByFechaAsync(fechaInicio, fechaFin);
                return Ok(facturas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving invoices by date", error = ex.Message });
            }
        }

        /// <summary>
        /// Creates an invoice from a delivered order
        /// </summary>
        [HttpPost("from-pedido/{pedidoId}")]
        public async Task<ActionResult<FacturaDTO>> CreateFacturaFromPedido(string pedidoId)
        {
            try
            {
                var factura = await _facturaService.CreateFacturaFromPedidoAsync(pedidoId);
                return CreatedAtAction(nameof(GetFactura), new { id = factura.Id }, factura);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating invoice", error = ex.Message });
            }
        }

        /// <summary>
        /// Updates invoice status
        /// </summary>
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> UpdateEstadoFactura(Guid id, [FromBody] ActualizarEstadoFacturaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var estado = Enum.Parse<EstadoFactura>(dto.Estado, ignoreCase: true);
                var result = await _facturaService.UpdateEstadoFacturaAsync(id, estado);
                
                if (!result) 
                    return NotFound(new { message = "Factura no encontrada" });
                
                return NoContent();
            }
            catch (ArgumentException)
            {
                return BadRequest(new { message = "Estado de factura inválido" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating invoice status", error = ex.Message });
            }
        }

        /// <summary>
        /// Generates and downloads invoice PDF
        /// </summary>
        [HttpGet("{id}/pdf")]
        public async Task<ActionResult> GetFacturaPDF(Guid id)
        {
            try
            {
                var pdf = await _facturaService.GenerarFacturaPDFAsync(id);
                var factura = await _facturaService.GetFacturaByIdAsync(id);
                
                var fileName = $"Factura_{factura?.NumeroFactura ?? id.ToString()}.pdf";
                
                return File(pdf, "application/pdf", fileName);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating PDF", error = ex.Message });
            }
        }

        /// <summary>
        /// Preview invoice PDF in browser
        /// </summary>
        [HttpGet("{id}/pdf/preview")]
        public async Task<ActionResult> PreviewFacturaPDF(Guid id)
        {
            try
            {
                var pdf = await _facturaService.GenerarFacturaPDFAsync(id);
                
                // Return PDF for inline viewing
                Response.Headers.Add("Content-Disposition", "inline");
                return File(pdf, "application/pdf");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating PDF preview", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets the next invoice number that would be generated
        /// </summary>
        [HttpGet("next-number")]
        public async Task<ActionResult<string>> GetNextInvoiceNumber()
        {
            try
            {
                var nextNumber = await _facturaService.GenerarNumeroFacturaAsync();
                return Ok(new { nextNumber });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating next invoice number", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets invoice statistics
        /// </summary>
        [HttpGet("estadisticas")]
        public async Task<ActionResult> GetEstadisticasFacturas([FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
        {
            try
            {
                var inicio = fechaInicio ?? DateTime.Now.AddMonths(-1);
                var fin = fechaFin ?? DateTime.Now;

                var facturas = await _facturaService.GetFacturasByFechaAsync(inicio, fin);
                var facturasList = facturas.ToList();

                var estadisticas = new
                {
                    TotalFacturas = facturasList.Count,
                    TotalMonto = facturasList.Sum(f => f.Total),
                    TotalIVA = facturasList.Sum(f => f.IVA),
                    PromedioFactura = facturasList.Any() ? facturasList.Average(f => f.Total) : 0,
                    FacturasPendientes = facturasList.Count(f => f.Estado == "Pendiente"),
                    FacturasPagadas = facturasList.Count(f => f.Estado == "Pagado"),
                    FacturasAnuladas = facturasList.Count(f => f.Estado == "Anulado"),
                    FacturasVencidas = facturasList.Count(f => f.Estado == "Vencido"),
                    PeriodoConsulta = new { Inicio = inicio, Fin = fin }
                };

                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating invoice statistics", error = ex.Message });
            }
        }
    }
}