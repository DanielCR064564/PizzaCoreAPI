using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Interfaces;
using PizzaCoreAPI.Models;
using PizzaCoreAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PizzaCoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncController : ControllerBase
    {
        private readonly IProductoService _productoService;
        private readonly IPedidoService _pedidoService;
        private readonly IVentaService _ventaService;
        private readonly ILogger<SyncController> _logger;
        private readonly ProductoSyncService _productoSyncService;
        private readonly IFacturaService _facturaService;

        public SyncController(
            IProductoService productoService,
            IPedidoService pedidoService,
            IVentaService ventaService,
            ILogger<SyncController> logger,
            ProductoSyncService productoSyncService,
            IFacturaService facturaService)
        {
            _productoService = productoService;
            _pedidoService = pedidoService;
            _ventaService = ventaService;
            _logger = logger;
            _productoSyncService = productoSyncService;
            _facturaService = facturaService;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new
            {
                status = "online",
                timestamp = DateTime.UtcNow,
                apiVersion = "1.0"
            });
        }

        [HttpGet("productos/all")]
        public async Task<IActionResult> GetAllProductos()
        {
            try
            {
                var productos = await _productoSyncService.GetAllProductosForSyncAsync();
                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products for sync");
                return StatusCode(500, new { error = "Error retrieving products" });
            }
        }

        [HttpGet("productos/disponibles")]
        public async Task<IActionResult> GetAvailableProductos()
        {
            try
            {
                var productos = await _productoSyncService.GetAllProductosForSyncAsync();
                var disponibles = productos.Where(p => p.Disponible).ToList();
                
                _logger.LogInformation($"Returning {disponibles.Count} available products");
                return Ok(disponibles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available products for sync");
                return StatusCode(500, new { error = "Error retrieving available products" });
            }
        }

        [HttpGet("productos/tipo/{tipo}")]
        public async Task<IActionResult> GetProductosByTipo(string tipo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tipo))
                {
                    return BadRequest(new { error = "Tipo de producto requerido" });
                }

                var productos = await _productoSyncService.GetAllProductosForSyncAsync();
                var productosPorTipo = productos.Where(p => 
                    p.Tipo.Equals(tipo, StringComparison.OrdinalIgnoreCase) && p.Disponible
                ).ToList();
                
                _logger.LogInformation($"Returning {productosPorTipo.Count} products of type '{tipo}'");
                return Ok(productosPorTipo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products by type: {Tipo}", tipo);
                return StatusCode(500, new { error = $"Error retrieving products of type {tipo}" });
            }
        }

        [HttpGet("productos/{id}")]
        public async Task<IActionResult> GetProductoById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest(new { error = "ID de producto requerido" });
                }

                var productos = await _productoSyncService.GetAllProductosForSyncAsync();
                var producto = productos.FirstOrDefault(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
                
                if (producto == null)
                {
                    return NotFound(new { error = $"Producto con ID {id} no encontrado" });
                }
                
                _logger.LogInformation($"Returning product: {producto.Nombre}");
                return Ok(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product by ID: {ProductId}", id);
                return StatusCode(500, new { error = $"Error retrieving product {id}" });
            }
        }

        [HttpGet("productos/tipos")]
        public async Task<IActionResult> GetProductTypes()
        {
            try
            {
                var productos = await _productoSyncService.GetAllProductosForSyncAsync();
                var tipos = productos
                    .Where(p => !string.IsNullOrWhiteSpace(p.Tipo))
                    .Select(p => p.Tipo)
                    .Distinct()
                    .OrderBy(t => t)
                    .ToList();
                
                _logger.LogInformation($"Returning {tipos.Count} product types");
                return Ok(new { tipos = tipos });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product types");
                return StatusCode(500, new { error = "Error retrieving product types" });
            }
        }

        [HttpGet("productos/stats")]
        public async Task<IActionResult> GetProductStats()
        {
            try
            {
                var productos = await _productoSyncService.GetAllProductosForSyncAsync();
                
                var stats = new
                {
                    total = productos.Count(),
                    disponibles = productos.Count(p => p.Disponible),
                    noDisponibles = productos.Count(p => !p.Disponible),
                    porTipo = productos.GroupBy(p => p.Tipo)
                        .ToDictionary(g => g.Key, g => new {
                            total = g.Count(),
                            disponibles = g.Count(p => p.Disponible)
                        }),
                    ultimaActualizacion = productos.Max(p => p.FechaActualizacion)
                };
                
                _logger.LogInformation("Returning product statistics");
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product statistics");
                return StatusCode(500, new { error = "Error retrieving product statistics" });
            }
        }

        // Search products by name or description
        [HttpGet("productos/buscar")]
        public async Task<IActionResult> SearchProductos([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new { error = "Query de búsqueda requerido" });
                }

                var productos = await _productoSyncService.GetAllProductosForSyncAsync();
                var resultados = productos.Where(p => 
                    (p.Nombre.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    p.Descripcion.Contains(query, StringComparison.OrdinalIgnoreCase)) &&
                    p.Disponible
                ).ToList();
                
                _logger.LogInformation($"Search '{query}' returned {resultados.Count} results");
                return Ok(resultados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products with query: {Query}", query);
                return StatusCode(500, new { error = "Error en la búsqueda de productos" });
            }
        }

        [HttpPost("ventas")]
        public async Task<IActionResult> ReceiveVenta([FromBody] SyncVentaDTO venta)
        {
            try
            {
                if (venta == null || venta.Detalles == null || !venta.Detalles.Any())
                {
                    return BadRequest(new { success = false, error = "Invalid sale data" });
                }

                // Enhanced logging for debugging
                _logger.LogInformation($"Receiving venta sync - Order: {venta.NumeroOrden}, MetodoPago: '{venta.MetodoPago}', TerminalId: '{venta.TerminalId}'");
                _logger.LogInformation($"Detalles count: {venta.Detalles.Count}");

                foreach (var detalle in venta.Detalles)
                {
                    _logger.LogInformation($"ProductoApiId: '{detalle.ProductoApiId}', Cantidad: {detalle.Cantidad}, Precio: {detalle.PrecioUnitario}");
                }

                // Validate TerminalId format if provided
                if (!string.IsNullOrEmpty(venta.TerminalId) && !Guid.TryParse(venta.TerminalId, out _))
                {
                    _logger.LogWarning($"Invalid TerminalId format: {venta.TerminalId}");
                }

                var result = await _ventaService.CreateVentaFromSyncAsync(venta);

                _logger.LogInformation($"Venta created successfully with ID: {result.Id}");

                return Ok(new
                {
                    success = true,
                    ventaId = result.Id.ToString(), // Convert Guid to string for response
                    pedidoId = result.PedidoId,
                    numeroOrden = result.NumeroOrden
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Business logic error for order: {NumeroOrden}", venta?.NumeroOrden ?? "Unknown");
                return BadRequest(new { success = false, error = ex.Message, type = "business_error" });
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Format error for order: {NumeroOrden}", venta?.NumeroOrden ?? "Unknown");
                return BadRequest(new { success = false, error = "Invalid data format", details = ex.Message, type = "format_error" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error for order: {NumeroOrden}", venta?.NumeroOrden ?? "Unknown");
                return BadRequest(new { success = false, error = "Internal server error", type = "server_error" });
            }
        }

        [HttpPost("ventas/batch")]
        public async Task<IActionResult> ReceiveBatchVentas([FromBody] List<SyncVentaDTO> ventas)
        {
            var results = new List<object>();

            foreach (var venta in ventas)
            {
                try
                {
                    var response = await ReceiveVenta(venta);
                    if (response is OkObjectResult okResult)
                    {
                        results.Add(new { venta.NumeroOrden, result = okResult.Value });
                    }
                }
                catch (Exception ex)
                {
                    results.Add(new { venta.NumeroOrden, error = ex.Message });
                }
            }

            return Ok(new { processed = results.Count, results });
        }

        [HttpPost("facturas")]
        public async Task<IActionResult> SyncFactura([FromBody] SyncFacturaDTO factura)
        {
            try
            {
                if (factura == null)
                {
                    return BadRequest(new { success = false, error = "Invalid factura data" });
                }

                _logger.LogInformation($"Receiving factura sync - ID: {factura.IdFactura}, Total: {factura.Total}, Estado: '{factura.Estado}'");

                var result = await _facturaService.SyncFacturaAsync(factura);

                _logger.LogInformation($"Factura synced successfully with ID: {result.Id}");

                return Ok(new
                {
                    success = true,
                    facturaId = result.Id,
                    numeroFactura = result.NumeroFactura,
                    estado = result.Estado
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Business logic error for factura: {FacturaId}", factura?.IdFactura ?? 0);
                return BadRequest(new { success = false, error = ex.Message, type = "business_error" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error for factura: {FacturaId}", factura?.IdFactura ?? 0);
                return StatusCode(500, new { success = false, error = "Internal server error", type = "server_error" });
            }
        }

        [HttpPost("facturas/batch")]
        public async Task<IActionResult> SyncFacturasBatch([FromBody] SyncFacturaBatchDTO batch)
        {
            try
            {
                if (batch == null || batch.Facturas == null || !batch.Facturas.Any())
                {
                    return BadRequest(new { success = false, error = "Invalid batch data" });
                }

                _logger.LogInformation($"Receiving batch factura sync - Count: {batch.Facturas.Count}, Terminal: '{batch.TerminalId}'");

                var result = await _facturaService.SyncFacturasBatchAsync(batch.Facturas);

                _logger.LogInformation($"Batch sync completed - Synced: {result.Synced}, Updated: {result.Updated}, Failed: {result.Failed}");

                return Ok(new
                {
                    success = true,
                    result = result,
                    terminalId = batch.TerminalId,
                    timestamp = batch.SyncTimestamp
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in batch factura sync");
                return StatusCode(500, new { 
                    success = false, 
                    error = "Internal server error", 
                    type = "server_error",
                    details = ex.Message 
                });
            }
        }

        [HttpGet("facturas/pending")]
        public async Task<IActionResult> GetPendingFacturas([FromQuery] DateTime? lastSyncDate)
        {
            try
            {
                _logger.LogInformation($"Getting pending facturas since: {lastSyncDate?.ToString() ?? "all"}");

                var facturas = await _facturaService.GetFacturasForSyncAsync(lastSyncDate);

                return Ok(new
                {
                    success = true,
                    count = facturas.Count(),
                    facturas = facturas
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending facturas");
                return StatusCode(500, new { 
                    success = false, 
                    error = "Internal server error", 
                    type = "server_error" 
                });
            }
        }

        // Add a status endpoint to check sync health
        [HttpGet("facturas/status")]
        public async Task<IActionResult> GetSyncStatus()
        {
            try
            {
                var stats = await _facturaService.GetEstadisticasAsync(
                    DateTime.UtcNow.AddDays(-30), 
                    DateTime.UtcNow
                );

                return Ok(new
                {
                    success = true,
                    totalFacturas = stats.TotalFacturas,
                    pendientes = stats.FacturasPendientes,
                    pagadas = stats.FacturasPagadas,
                    lastSync = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sync status");
                return StatusCode(500, new { 
                    success = false, 
                    error = "Internal server error", 
                    type = "server_error" 
                });
            }
        }

    }

    // DTOs for synchronization
    public class ProductoSyncDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public bool Disponible { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public DateTime FechaActualizacion { get; set; }
    }
}