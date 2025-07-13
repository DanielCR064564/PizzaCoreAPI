using Microsoft.AspNetCore.Mvc;
using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Services;
using System.Threading.Tasks;

namespace PizzaCoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CuentasPorCobrarController : ControllerBase
    {
        private readonly ICuentasPorCobrarService _service;
        public CuentasPorCobrarController(ICuentasPorCobrarService service) { _service = service; }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var cuenta = await _service.GetByIdAsync(id);
            if (cuenta == null) return NotFound();
            return Ok(cuenta);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearCuentasPorCobrarDTO dto)
        {
            var cuenta = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = cuenta.Id }, cuenta);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CrearCuentasPorCobrarDTO dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
