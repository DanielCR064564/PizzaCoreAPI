using Microsoft.AspNetCore.Mvc;
using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Interfaces;
using PizzaCoreAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaCoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientesController : ControllerBase
    {
        private readonly IIngredienteService _ingredienteService;

        public IngredientesController(IIngredienteService ingredienteService)
        {
            _ingredienteService = ingredienteService;
        }

        // GET: api/Ingredientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IngredienteDTO>>> GetIngredientes()
        {
            var ingredientes = await _ingredienteService.GetAllIngredientesAsync();
            return Ok(ingredientes);
        }

        // GET: api/Ingredientes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<IngredienteDTO>> GetIngrediente(Guid id)
        {
            var ingrediente = await _ingredienteService.GetIngredienteByIdAsync(id);
            if (ingrediente == null)
                return NotFound();

            return Ok(ingrediente);
        }

        // POST: api/Ingredientes
        [HttpPost]
        public async Task<ActionResult<IngredienteDTO>> PostIngrediente([FromBody] CrearIngredienteDTO ingredienteDto)
        {
            try
            {
                var ingrediente = await _ingredienteService.CreateIngredienteAsync(ingredienteDto);
                return CreatedAtAction(nameof(GetIngrediente), new { id = ingrediente.Id }, ingrediente);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { 
                    message = $"Invalid tipo value. Use GET /api/Ingredientes/tipos to see all valid values", 
                    error = ex.Message 
                });
            }
        }

        // PUT: api/Ingredientes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIngrediente(Guid id, [FromBody] CrearIngredienteDTO ingredienteDto)
        {
            try
            {
                await _ingredienteService.UpdateIngredienteAsync(id, ingredienteDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { 
                    message = $"Invalid tipo value. Use GET /api/Ingredientes/tipos to see all valid values", 
                    error = ex.Message 
                });
            }
        }

        // DELETE: api/Ingredientes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngrediente(Guid id)
        {
            await _ingredienteService.DeleteIngredienteAsync(id);
            return NoContent();
        }

        // GET: api/Ingredientes/tipos - Helper endpoint to see available types
        [HttpGet("tipos")]
        public IActionResult GetTiposIngrediente()
        {
            var tipos = Enum.GetValues<TipoIngrediente>()
                .Select(t => new { Value = (int)t, Name = t.ToString() });
            return Ok(tipos);
        }
    }
}