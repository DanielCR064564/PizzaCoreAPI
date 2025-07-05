using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaCoreAPI.Data;
using PizzaCoreAPI.Models;
using System.Text;

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
        public async Task<IActionResult> GenerarFactura([FromBody] string pedidoId)
        {
            try
            {
                var detalles = await _context.PedidoDetalles
                    .Include(pd => pd.Producto)
                    .Where(pd => pd.PedidoId == pedidoId)
                    .Select(pd => new { pd.Cantidad, pd.PrecioUnitario, pd.Subtotal, pd.Producto })
                    .ToListAsync();

                var pedido = await _context.Pedidos
                    .Include(p => p.Cliente)
                    .Include(p => p.Empleado)
                    .FirstOrDefaultAsync(p => p.Id == pedidoId);

                if (pedido == null)
                    return NotFound();

                var subtotal = detalles.Sum(d => d.Subtotal);
                var iva = subtotal * 0.18m;
                var total = subtotal + iva;

                var factura = new Factura
                {
                    PedidoId = pedidoId,
                    NumeroFactura = "FAC-" + pedidoId,
                    RNC = pedido.Cliente?.RNC ?? string.Empty,
                    Subtotal = subtotal,
                    IVA = iva,
                    Total = total,
                    Estado = "Pendiente",
                    NombreCliente = pedido.Cliente?.NombreCompleto ?? string.Empty,
                    DireccionCliente = pedido.Cliente?.Direccion ?? string.Empty,
                    NombreEmpleado = pedido.Empleado?.NombreCompleto ?? string.Empty,
                    RNCEmpleado = pedido.Empleado?.RNC ?? string.Empty,
                    FechaEmision = DateTime.Now,
                    Detalles = detalles.Select(d => new PedidoDetalle
                    {
                        Cantidad = d.Cantidad, // ✅ corregido: se queda como int
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal,
                        Producto = d.Producto
                    }).ToList()
                };

                await _context.Facturas.AddAsync(factura);
                await _context.SaveChangesAsync();

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
            var html = new StringBuilder();

            html.Append("<html><head><style>")
                .Append("body { font-family: Arial, sans-serif; }")
                .Append(".factura { max-width: 800px; margin: 0 auto; padding: 20px; }")
                .Append(".header { text-align: center; margin-bottom: 30px; }")
                .Append(".detail { margin: 20px 0; }")
                .Append(".table { width: 100%; border-collapse: collapse; margin: 20px 0; }")
                .Append(".table th, .table td { border: 1px solid #ddd; padding: 8px; }")
                .Append(".table th { background-color: #f4f4f4; }")
                .Append(".total { text-align: right; margin-top: 20px; }")
                .Append("</style></head><body>")
                .Append("<div class='factura'>")
                .Append("<div class='header'><h1>FACTURA</h1>")
                .Append($"<p>Número: {factura?.NumeroFactura ?? "No especificado"}</p>")
                .Append($"<p>Fecha: {factura.FechaEmision:dd/MM/yyyy}</p></div>")
                .Append("<div class='detail'><h3>Datos del Cliente</h3>")
                .Append($"<p>Nombre: {factura?.NombreCliente ?? "No especificado"}</p>")
                .Append($"<p>RNC: {factura?.RNC ?? "No especificado"}</p>")
                .Append($"<p>Dirección: {factura?.DireccionCliente ?? "No especificada"}</p></div>")
                .Append("<div class='detail'><h3>Datos del Empleado</h3>")
                .Append($"<p>Nombre: {factura?.NombreEmpleado ?? "No especificado"}</p>")
                .Append($"<p>RNC: {factura?.RNCEmpleado ?? "No especificado"}</p></div>")
                .Append("<table class='table'><thead><tr>")
                .Append("<th>Producto</th><th>Cantidad</th><th>Precio Unitario</th><th>Subtotal</th>")
                .Append("</tr></thead><tbody>");

            if (factura.Detalles != null)
            {
                foreach (var detalle in factura.Detalles)
                {
                    html.Append("<tr>")
                        .Append($"<td>{detalle.Producto?.Nombre ?? "No especificado"}</td>")
                        .Append($"<td>{detalle.Cantidad}</td>")
                        .Append($"<td>{detalle.PrecioUnitario:C}</td>")
                        .Append($"<td>{detalle.Subtotal:C}</td>")
                        .Append("</tr>");
                }
            }

            html.Append("</tbody></table>")
                .Append("<div class='total'>")
                .Append($"<p>Subtotal: {factura?.Subtotal:C}</p>")
                .Append($"<p>IVA (18%): {factura?.IVA:C}</p>")
                .Append($"<h3>Total: {factura?.Total:C}</h3>")
                .Append("</div></div></body></html>");

            return html.ToString();
        }
    }
}
