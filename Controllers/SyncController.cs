using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Interfaces;
using PizzaCoreAPI.Models;
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

        public SyncController(
            IProductoService productoService,
            IPedidoService pedidoService,
            IVentaService ventaService,
            ILogger<SyncController> logger)
        {
            _productoService = productoService;
            _pedidoService = pedidoService;
            _ventaService = ventaService;
            _logger = logger;
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
                var productos = await _productoService.GetAllProductosAsync();
                var response = productos.Select(p => new ProductoSyncDTO
                {
                    Id = p.Id.ToString(),
                    Nombre = p.Nombre,
                    Precio = p.Precio,
                    Descripcion = p.Descripcion,
                    Disponible = p.Disponible,
                    Tipo = p.Tipo,
                    FechaActualizacion = p.FechaActualizacion
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products for sync");
                return StatusCode(500, new { error = "Error retrieving products" });
            }
        }

        [HttpPost("ventas")]
        public async Task<IActionResult> ReceiveVenta([FromBody] SyncVentaDTO venta)
        {
            try
            {
                // Validate input
                if (venta == null || venta.Detalles == null || !venta.Detalles.Any())
                {
                    return BadRequest(new { success = false, error = "Invalid sale data" });
                }

                // Create the sale using the new VentaService
                var result = await _ventaService.CreateVentaFromSyncAsync(venta);

                return Ok(new
                {
                    success = true,
                    ventaId = result.Id,
                    pedidoId = result.PedidoId,
                    numeroOrden = result.NumeroOrden
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error receiving sale from POS");
                return BadRequest(new { success = false, error = ex.Message });
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

        private MetodoPago ConvertMetodoPago(string metodoPago)
        {
            return metodoPago?.ToLower() switch
            {
                "efectivo" => MetodoPago.Efectivo,
                "tarjeta" => MetodoPago.Tarjeta,
                "transferencia" => MetodoPago.Transferencia,
                _ => MetodoPago.Efectivo
            };
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