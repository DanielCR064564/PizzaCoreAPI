using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using PizzaCoreAPI.Data;
using PizzaCoreAPI.Models;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace PizzaCoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturaController : ControllerBase
    {
        private readonly PizzaDbContext _context;
        private readonly IConverter _converter;

        public FacturaController(PizzaDbContext context, IConverter converter)
        {
            _context = context;
            _converter = converter;
        }

        // POST: api/Factura/Generar
        [HttpPost("Generar")]
        public async Task<IActionResult> GenerarFactura([FromBody] int pedidoId)
        {
            try
            {
                var detalles = await _context.PedidoDetalles
                    .Include(pd => pd.Producto)
                    .ToListAsync();

                var pedido = await _context.Pedidos
                    .Include(p => p.Cliente)
                    .Include(p => p.Empleado)
                    .FirstOrDefaultAsync(p => p.Id == pedidoId);

                if (pedido == null)
                    return NotFound();

                // Crear la factura
                var factura = new Factura
                {
                    PedidoId = pedido.Id,
                    NumeroFactura = "FAC-" + pedido.Id.ToString("D6"),
                    RNC = pedido.Cliente?.RNC ?? string.Empty,
                    Subtotal = pedido.Detalles.Sum(d => d.Subtotal),
                    IVA = pedido.Detalles.Sum(d => d.Subtotal) * 0.18m,
                    Total = pedido.Detalles.Sum(d => d.Subtotal) * 1.18m,
                    Estado = "Pendiente",
                    NombreCliente = pedido.Cliente?.NombreCompleto ?? string.Empty,
                    DireccionCliente = pedido.Cliente?.Direccion ?? string.Empty,
                    NombreEmpleado = pedido.Empleado?.NombreCompleto ?? string.Empty,
                    RNCEmpleado = pedido.Empleado?.RNC ?? string.Empty,
                    FechaEmision = DateTime.Now,
                    Detalles = pedido.Detalles ?? new List<PedidoDetalle>()
                };

                // Guardar la factura en la base de datos
                await _context.Facturas.AddAsync(factura);
                await _context.SaveChangesAsync();

                // Generar PDF
                var document = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Portrait,
                        PaperSize = PaperKind.A4,
                        Margins = new MarginSettings { Top = 10 }
                    },
                    Objects = {
                        new ObjectSettings() {
                            PagesCount = true,
                            HtmlContent = await GenerateHtmlFactura(factura),
                            WebSettings = { DefaultEncoding = "utf-8" }
                        }
                    }
                };

                var pdf = _converter.Convert(document);

                return File(pdf, "application/pdf", $"factura_{factura.NumeroFactura}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al generar factura", error = ex.Message });
            }
        }

        private async Task<string> GenerateHtmlFactura(Factura factura)
        {
            var html = $""
                + "<html>"
                + "<head>"
                + "<style>"
                + "body {{ font-family: Arial, sans-serif; }}"
                + ".factura {{ max-width: 800px; margin: 0 auto; padding: 20px; }}"
                + ".header {{ text-align: center; margin-bottom: 30px; }}"
                + ".detail {{ margin: 20px 0; }}"
                + ".table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}"
                + ".table th, .table td {{ border: 1px solid #ddd; padding: 8px; }}"
                + ".table th {{ background-color: #f4f4f4; }}"
                + ".total {{ text-align: right; margin-top: 20px; }}"
                + "</style>"
                + "</head>"
                + "<body>"
                + $"<div class='factura'>"
                + $"<div class='header'>"
                + $"<h1>FACTURA</h1>"
                + $"<p>Número: {factura?.NumeroFactura ?? "No especificado"}</p>"
                + $"<p>Fecha: {factura.FechaEmision.ToString("dd/MM/yyyy")}</p>"
                + $"</div>"
                + $"<div class='detail'>"
                + $"<h3>Datos del Cliente</h3>"
                + $"<p>Nombre: {factura?.NombreCliente ?? "No especificado"}</p>"
                + $"<p>RNC: {factura?.RNC ?? "No especificado"}</p>"
                + $"<p>Dirección: {factura?.DireccionCliente ?? "No especificada"}</p>"
                + $"</div>"
                + $"<div class='detail'>"
                + $"<h3>Datos del Empleado</h3>"
                + $"<p>Nombre: {factura?.NombreEmpleado ?? "No especificado"}</p>"
                + $"<p>RNC: {factura?.RNCEmpleado ?? "No especificado"}</p>"
                + $"</div>"
                + $"<table class='table'>"
                + $"<thead>"
                + $"<tr>"
                + $"<th>Producto</th>"
                + $"<th>Cantidad</th>"
                + $"<th>Precio Unitario</th>"
                + $"<th>Subtotal</th>"
                + $"</tr>"
                + $"</thead>"
                + $"<tbody>";

            if (factura.Detalles != null)
            {
                foreach (var detalle in factura.Detalles)
                {
                    html += $"<tr>"
                        + $"<td>{detalle.Producto?.Nombre ?? "No especificado"}</td>"
                        + $"<td>{detalle.Cantidad}</td>"
                        + $"<td>{detalle.PrecioUnitario:C}</td>"
                        + $"<td>{detalle.Subtotal:C}</td>"
                        + $"</tr>";
                }
            }

            html += $"</tbody>"
                + $"</table>"
                + $"<div class='total'>"
                + $"<p>Subtotal: {factura?.Subtotal:C}</p>"
                + $"<p>IVA (18%): {factura?.IVA:C}</p>"
                + $"<h3>Total: {factura?.Total:C}</h3>"
                + $"</div>"
                + $"</div>"
                + "</body>"
                + "</html>";

            return html;
        }
    }
}
