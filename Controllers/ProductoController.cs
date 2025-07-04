using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Interfaces;

namespace PizzaCoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductoDTO>>> GetProductos()
        {
            var productos = await _productoService.GetAllProductosAsync();
            return Ok(productos);
        }

        [HttpGet("disponibles")]
        public async Task<ActionResult<List<ProductoDTO>>> GetProductosDisponibles()
        {
            var productos = await _productoService.GetProductosDisponiblesAsync();
            return Ok(productos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDTO>> GetProducto(Guid id)
        {
            var producto = await _productoService.GetProductoByIdAsync(id);
            if (producto == null)
                return NotFound();
            return Ok(producto);
        }

        [HttpPost]
        public async Task<ActionResult<ProductoDTO>> CreateProducto(CrearProductoDTO productoDto)
        {
            var producto = await _productoService.CreateProductoAsync(productoDto);
            return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProducto(Guid id, CrearProductoDTO productoDto)
        {
            await _productoService.UpdateProductoAsync(id, productoDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(Guid id)
        {
            await _productoService.DeleteProductoAsync(id);
            return NoContent();
        }

        [HttpGet("{id}/precio")]
        public async Task<ActionResult<decimal>> CalcularPrecio(Guid id, [FromQuery] List<Guid> ingredientesIds)
        {
            var precio = await _productoService.CalcularPrecioConIngredientesAsync(id, ingredientesIds);
            return Ok(precio);
        }

        [HttpGet("{id}/disponibilidad")]
        public async Task<ActionResult<bool>> VerificarDisponibilidad(Guid id)
        {
            var disponible = await _productoService.VerificarDisponibilidadAsync(id);
            return Ok(disponible);
        }
    }
}
