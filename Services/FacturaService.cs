using Microsoft.EntityFrameworkCore;
using PizzaCoreAPI.Data;
using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Models;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace PizzaCoreAPI.Services
{
    public class FacturaService : IFacturaService
    {
        private readonly PizzaDbContext _context;
        private readonly IConverter _converter;
        private const decimal IVA_RATE = 0.18m; // 18% IVA for Dominican Republic

        public FacturaService(PizzaDbContext context, IConverter converter)
        {
            _context = context;
            _converter = converter;
        }

        public async Task<IEnumerable<FacturaDTO>> GetAllFacturasAsync()
        {
            var facturas = await _context.Facturas
                .Include(f => f.Pedido)
                .ThenInclude(p => p.Cliente)
                .Include(f => f.Pedido)
                .ThenInclude(p => p.Empleado)
                .Include(f => f.Pedido)
                .ThenInclude(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .OrderByDescending(f => f.FechaEmision)
                .ToListAsync();

            return facturas.Select(MapToDTO);
        }

        public async Task<FacturaDTO?> GetFacturaByIdAsync(Guid id)
        {
            var factura = await _context.Facturas
                .Include(f => f.Pedido)
                .ThenInclude(p => p.Cliente)
                .Include(f => f.Pedido)
                .ThenInclude(p => p.Empleado)
                .Include(f => f.Pedido)
                .ThenInclude(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(f => f.Id == id);

            return factura != null ? MapToDTO(factura) : null;
        }

        public async Task<FacturaDTO?> GetFacturaByPedidoIdAsync(string pedidoId)
        {
            var factura = await _context.Facturas
                .Include(f => f.Pedido)
                .ThenInclude(p => p.Cliente)
                .Include(f => f.Pedido)
                .ThenInclude(p => p.Empleado)
                .Include(f => f.Pedido)
                .ThenInclude(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(f => f.PedidoId == pedidoId);

            return factura != null ? MapToDTO(factura) : null;
        }

        public async Task<IEnumerable<FacturaDTO>> GetFacturasByEstadoAsync(EstadoFactura estado)
        {
            var facturas = await _context.Facturas
                .Include(f => f.Pedido)
                .ThenInclude(p => p.Cliente)
                .Include(f => f.Pedido)
                .ThenInclude(p => p.Empleado)
                .Include(f => f.Pedido)
                .ThenInclude(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .Where(f => f.Estado == estado)
                .OrderByDescending(f => f.FechaEmision)
                .ToListAsync();

            return facturas.Select(MapToDTO);
        }

        public async Task<IEnumerable<FacturaDTO>> GetFacturasByFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var facturas = await _context.Facturas
                .Include(f => f.Pedido)
                .ThenInclude(p => p.Cliente)
                .Include(f => f.Pedido)
                .ThenInclude(p => p.Empleado)
                .Include(f => f.Pedido)
                .ThenInclude(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .Where(f => f.FechaEmision.Date >= fechaInicio.Date && 
                           f.FechaEmision.Date <= fechaFin.Date)
                .OrderByDescending(f => f.FechaEmision)
                .ToListAsync();

            return facturas.Select(MapToDTO);
        }

        public async Task<FacturaDTO> CreateFacturaFromPedidoAsync(string pedidoId)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Empleado)
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.Id == pedidoId);

            if (pedido == null)
                throw new InvalidOperationException("Pedido no encontrado");

            if (pedido.Estado != EstadoPedido.Entregado)
                throw new InvalidOperationException("Solo se pueden facturar pedidos entregados");

            // Check if invoice already exists
            var existingFactura = await _context.Facturas
                .FirstOrDefaultAsync(f => f.PedidoId == pedidoId);
            if (existingFactura != null)
                throw new InvalidOperationException("El pedido ya tiene una factura asociada");

            // Validate that order has details
            if (!pedido.Detalles.Any())
                throw new InvalidOperationException("El pedido no tiene productos para facturar");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Calculate taxes - IVA is included in the total price
                var totalConIVA = pedido.Total;
                var subtotal = totalConIVA / (1 + IVA_RATE);
                var iva = totalConIVA - subtotal;

                var factura = new Factura
                {
                    Id = Guid.NewGuid(),
                    PedidoId = pedidoId,
                    NumeroFactura = await GenerarNumeroFacturaAsync(),
                    RNC = pedido.Cliente?.RNC ?? "N/A",
                    Subtotal = Math.Round(subtotal, 2),
                    IVA = Math.Round(iva, 2),
                    Total = Math.Round(totalConIVA, 2),
                    Estado = EstadoFactura.Pendiente,
                    NombreCliente = pedido.Cliente?.NombreCompleto ?? "Cliente General",
                    DireccionCliente = pedido.Cliente?.Direccion ?? "N/A",
                    NombreEmpleado = pedido.Empleado?.NombreCompleto ?? "Sistema",
                    RNCEmpleado = pedido.Empleado?.RNC ?? "N/A",
                    FechaEmision = DateTime.Now
                };

                _context.Facturas.Add(factura);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetFacturaByIdAsync(factura.Id) ?? 
                       throw new InvalidOperationException("Error retrieving created invoice");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateEstadoFacturaAsync(Guid id, EstadoFactura estado)
        {
            var factura = await _context.Facturas.FindAsync(id);
            if (factura == null) return false;

            // Business rules for state transitions
            if (!ValidarTransicionEstadoFactura(factura.Estado, estado))
                throw new InvalidOperationException($"No se puede cambiar el estado de {factura.Estado} a {estado}");

            factura.Estado = estado;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<byte[]> GenerarFacturaPDFAsync(Guid facturaId)
        {
            var factura = await GetFacturaByIdAsync(facturaId);
            if (factura == null)
                throw new InvalidOperationException("Factura no encontrada");

            var html = GenerarHTMLFactura(factura);

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 }
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = html,
                        WebSettings = { DefaultEncoding = "utf-8" },
                        HeaderSettings = { 
                            FontSize = 9, 
                            Right = "Página [page] de [toPage]", 
                            Line = true,
                            Spacing = 2.812
                        },
                        FooterSettings = { 
                            FontSize = 9, 
                            Line = true, 
                            Center = "Pizza Restaurant - Sistema de Facturación",
                            Spacing = 2.812
                        }
                    }
                }
            };

            return _converter.Convert(doc);
        }

        public async Task<string> GenerarNumeroFacturaAsync()
        {
            var lastFactura = await _context.Facturas
                .OrderByDescending(f => f.FechaEmision)
                .ThenByDescending(f => f.NumeroFactura)
                .FirstOrDefaultAsync();

            var nextNumber = 1;
            if (lastFactura != null && lastFactura.NumeroFactura.StartsWith("FAC-"))
            {
                var numberPart = lastFactura.NumeroFactura.Substring(4);
                if (int.TryParse(numberPart, out var currentNumber))
                {
                    nextNumber = currentNumber + 1;
                }
            }

            return $"FAC-{nextNumber:D8}"; // 8-digit number with leading zeros
        }

        private static bool ValidarTransicionEstadoFactura(EstadoFactura estadoActual, EstadoFactura nuevoEstado)
        {
            return estadoActual switch
            {
                EstadoFactura.Pendiente => nuevoEstado is EstadoFactura.Pagado or EstadoFactura.Anulado or EstadoFactura.Vencido,
                EstadoFactura.Pagado => nuevoEstado is EstadoFactura.Anulado, // Only allow cancellation of paid invoices
                EstadoFactura.Vencido => nuevoEstado is EstadoFactura.Pagado or EstadoFactura.Anulado,
                EstadoFactura.Anulado => false, // Cannot change from cancelled
                _ => false
            };
        }

        private static string GenerarHTMLFactura(FacturaDTO factura)
        {
            var detallesHtml = string.Join("", factura.Detalles.Select(d => $@"
            <tr>
                <td style='padding: 8px; border-bottom: 1px solid #ddd;'>{d.Producto.Nombre}</td>
                <td style='padding: 8px; border-bottom: 1px solid #ddd; text-align: center;'>{d.Cantidad}</td>
                <td style='padding: 8px; border-bottom: 1px solid #ddd; text-align: right;'>RD${d.PrecioUnitario:F2}</td>
                <td style='padding: 8px; border-bottom: 1px solid #ddd; text-align: right;'>RD${d.Subtotal:F2}</td>
            </tr>"));

            return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Factura {factura.NumeroFactura}</title>
    <style>
        body {{ 
            font-family: 'Arial', sans-serif; 
            margin: 0; 
            padding: 20px; 
            color: #333;
            line-height: 1.4;
        }}
        .container {{ max-width: 800px; margin: 0 auto; }}
        .header {{ 
            text-align: center; 
            margin-bottom: 30px; 
            border-bottom: 3px solid #e74c3c;
            padding-bottom: 20px;
        }}
        .company-name {{ 
            font-size: 28px; 
            font-weight: bold; 
            color: #e74c3c; 
            margin: 0;
        }}
        .invoice-title {{ 
            font-size: 20px; 
            color: #2c3e50; 
            margin: 10px 0;
        }}
        .company-info {{ 
            font-size: 12px; 
            color: #666;
            margin-top: 10px;
        }}
        .invoice-info {{ 
            display: flex; 
            justify-content: space-between; 
            margin-bottom: 30px;
        }}
        .invoice-details, .client-info {{ 
            flex: 1; 
            margin-right: 20px;
        }}
        .client-info {{ margin-right: 0; }}
        .section-title {{ 
            font-size: 14px; 
            font-weight: bold; 
            color: #2c3e50; 
            margin-bottom: 10px;
            border-bottom: 1px solid #bdc3c7;
            padding-bottom: 5px;
        }}
        .info-line {{ margin: 5px 0; font-size: 12px; }}
        .items-table {{ 
            width: 100%; 
            border-collapse: collapse; 
            margin: 20px 0;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }}
        .items-table thead {{ background-color: #34495e; color: white; }}
        .items-table th {{ 
            padding: 12px 8px; 
            text-align: left; 
            font-size: 12px; 
            font-weight: bold;
        }}
        .items-table td {{ 
            padding: 8px; 
            border-bottom: 1px solid #ecf0f1; 
            font-size: 11px;
        }}
        .items-table tbody tr:nth-child(even) {{ background-color: #f8f9fa; }}
        .totals {{ 
            text-align: right; 
            margin-top: 30px;
            border-top: 2px solid #34495e;
            padding-top: 15px;
        }}
        .total-line {{ 
            margin: 8px 0; 
            font-size: 13px;
        }}
        .total-final {{ 
            font-size: 18px; 
            font-weight: bold; 
            color: #e74c3c;
            border-top: 2px solid #e74c3c;
            padding-top: 10px;
            margin-top: 10px;
        }}
        .footer {{ 
            margin-top: 40px; 
            text-align: center; 
            font-size: 10px; 
            color: #666;
            border-top: 1px solid #bdc3c7;
            padding-top: 15px;
        }}
        .status-badge {{
            display: inline-block;
            padding: 4px 8px;
            border-radius: 4px;
            font-size: 10px;
            font-weight: bold;
            color: white;
            background-color: {(factura.Estado == "Pagado" ? "#27ae60" : factura.Estado == "Pendiente" ? "#f39c12" : "#e74c3c")};
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 class='company-name'>PIZZA RESTAURANT</h1>
            <h2 class='invoice-title'>FACTURA DE CONSUMO</h2>
            <div class='company-info'>
                <div>RNC: 123-45678-9 | Tel: (809) 555-0123</div>
                <div>Dirección: Av. Principal #123, Santo Domingo, República Dominicana</div>
                <div>Email: info@pizzarestaurant.com</div>
            </div>
        </div>
        
        <div class='invoice-info'>
            <div class='invoice-details'>
                <div class='section-title'>DETALLES DE FACTURA</div>
                <div class='info-line'><strong>Número:</strong> {factura.NumeroFactura}</div>
                <div class='info-line'><strong>Fecha:</strong> {factura.FechaEmision:dd/MM/yyyy HH:mm}</div>
                <div class='info-line'><strong>Empleado:</strong> {factura.NombreEmpleado}</div>
                <div class='info-line'><strong>Estado:</strong> <span class='status-badge'>{factura.Estado.ToUpper()}</span></div>
            </div>
            
            <div class='client-info'>
                <div class='section-title'>DATOS DEL CLIENTE</div>
                <div class='info-line'><strong>Nombre:</strong> {factura.NombreCliente}</div>
                <div class='info-line'><strong>RNC/Cédula:</strong> {factura.RNC}</div>
                <div class='info-line'><strong>Dirección:</strong> {factura.DireccionCliente}</div>
            </div>
        </div>
        
        <table class='items-table'>
            <thead>
                <tr>
                    <th>Producto</th>
                    <th style='text-align: center; width: 10%;'>Cant.</th>
                    <th style='text-align: right; width: 15%;'>Precio Unit.</th>
                    <th style='text-align: right; width: 15%;'>Subtotal</th>
                </tr>
            </thead>
            <tbody>
                {detallesHtml}
            </tbody>
        </table>
        
        <div class='totals'>
            <div class='total-line'><strong>Subtotal (Sin IVA): RD${factura.Subtotal:F2}</strong></div>
            <div class='total-line'><strong>IVA ({factura.PorcentajeIVA:F1}%): RD${factura.IVA:F2}</strong></div>
            <div class='total-final'><strong>TOTAL A PAGAR: RD${factura.Total:F2}</strong></div>
        </div>
        
        <div class='footer'>
            <p><strong>¡Gracias por su preferencia!</strong></p>
            <p>Esta factura fue generada electrónicamente y es válida sin firma ni sello.</p>
            <p>Para cualquier consulta, contáctenos al (809) 555-0123 o info@pizzarestaurant.com</p>
        </div>
    </div>
</body>
</html>";
        }

        private static FacturaDTO MapToDTO(Factura factura)
        {
            return new FacturaDTO
            {
                Id = factura.Id,
                PedidoId = factura.PedidoId,
                NumeroFactura = factura.NumeroFactura,
                RNC = factura.RNC,
                Subtotal = factura.Subtotal,
                IVA = factura.IVA,
                Total = factura.Total,
                Estado = factura.Estado.ToString(),
                NombreCliente = factura.NombreCliente,
                DireccionCliente = factura.DireccionCliente,
                NombreEmpleado = factura.NombreEmpleado,
                RNCEmpleado = factura.RNCEmpleado,
                FechaEmision = factura.FechaEmision,
                Detalles = factura.Pedido?.Detalles?.Select(d => new PedidoDetalleDTO
                {
                    Id = d.Id,
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal,
                    Producto = new ProductoDTO
                    {
                        Id = d.Producto.Id,
                        Nombre = d.Producto.Nombre,
                        Descripcion = d.Producto.Descripcion,
                        Precio = d.Producto.Precio,
                        Tipo = d.Producto.Tipo.ToString(),
                        Disponible = d.Producto.Disponible
                    }
                }).ToList() ?? new List<PedidoDetalleDTO>()
            };
        }

        public Task<IEnumerable<FacturaDTO>> GetFacturasByClienteAsync(string clienteId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PuedeCrearFacturaAsync(string pedidoId)
        {
            throw new NotImplementedException();
        }

        public Task<EstadisticasFacturaDTO> GetEstadisticasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            throw new NotImplementedException();
        }
    }
}