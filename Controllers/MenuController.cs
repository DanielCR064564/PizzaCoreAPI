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
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        public async Task<ActionResult<List<MenuDTO>>> GetMenus()
        {
            var menus = await _menuService.GetAllMenusAsync();
            return Ok(menus);
        }

        [HttpGet("activos")]
        public async Task<ActionResult<List<MenuDTO>>> GetMenusActivos()
        {
            var menus = await _menuService.GetMenusActivosAsync();
            return Ok(menus);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MenuDTO>> GetMenu(Guid id)
        {
            var menu = await _menuService.GetMenuByIdAsync(id);
            if (menu == null)
                return NotFound();
            return Ok(menu);
        }

        [HttpPost]
        public async Task<ActionResult<MenuDTO>> CreateMenu(CrearMenuDTO menuDto)
        {
            var menu = await _menuService.CreateMenuAsync(menuDto);
            return CreatedAtAction(nameof(GetMenu), new { id = menu.Id }, menu);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenu(Guid id, CrearMenuDTO menuDto)
        {
            await _menuService.UpdateMenuAsync(id, menuDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenu(Guid id)
        {
            await _menuService.DeleteMenuAsync(id);
            return NoContent();
        }

        [HttpGet("{id}/precio")]
        public async Task<ActionResult<decimal>> CalcularPrecio(Guid id)
        {
            var precio = await _menuService.CalcularPrecioMenuAsync(id);
            return Ok(precio);
        }

        [HttpGet("{id}/disponibilidad")]
        public async Task<ActionResult<bool>> VerificarDisponibilidad(Guid id)
        {
            var disponible = await _menuService.VerificarDisponibilidadMenuAsync(id);
            return Ok(disponible);
        }
    }
}
