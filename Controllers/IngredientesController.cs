using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaCoreAPI.Data;
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
        private readonly PizzaDbContext _context;

        public IngredientesController(PizzaDbContext context)
        {
            _context = context;
        }

        // GET: api/Ingredientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ingrediente>>> GetIngredientes()
        {
            return await _context.Ingredientes.ToListAsync();
        }

        // GET: api/Ingredientes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Ingrediente>> GetIngrediente(Guid id)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);
            if (ingrediente == null)
                return NotFound();

            return ingrediente;
        }

        // POST: api/Ingredientes
        [HttpPost]
        public async Task<ActionResult<Ingrediente>> PostIngrediente(Ingrediente ingrediente)
        {
            _context.Ingredientes.Add(ingrediente);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetIngrediente", new { id = ingrediente.Id }, ingrediente);
        }

        // PUT: api/Ingredientes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIngrediente(Guid id, Ingrediente ingrediente)
        {
            if (id != ingrediente.Id)
                return BadRequest();

            _context.Entry(ingrediente).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Ingredientes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngrediente(Guid id)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);
            if (ingrediente == null)
                return NotFound();

            _context.Ingredientes.Remove(ingrediente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
