using Microsoft.AspNetCore.Mvc;
using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Services;
using System;
using System.Threading.Tasks;

namespace PizzaCoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetodoDePagoController : ControllerBase
    {
        private readonly IMetodoDePagoService _service;
        public MetodoDePagoController(IMetodoDePagoService service) { _service = service; }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var metodo = await _service.GetByIdAsync(id);
            if (metodo == null) return NotFound();
            return Ok(metodo);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearMetodoDePagoDTO dto)
        {
            var metodo = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = metodo.Id }, metodo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CrearMetodoDePagoDTO dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
