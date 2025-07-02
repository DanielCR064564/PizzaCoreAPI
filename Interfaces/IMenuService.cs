using System.Collections.Generic;
using System.Threading.Tasks;
using PizzaCoreAPI.Models;
using PizzaCoreAPI.DTOs;

namespace PizzaCoreAPI.Interfaces
{
    public interface IMenuService
    {
        Task<List<MenuDTO>> GetAllMenusAsync();
        Task<MenuDTO> GetMenuByIdAsync(int id);
        Task<MenuDTO> CreateMenuAsync(CrearMenuDTO menuDto);
        Task UpdateMenuAsync(int id, CrearMenuDTO menuDto);
        Task DeleteMenuAsync(int id);
        Task<List<MenuDTO>> GetMenusActivosAsync();
        Task<decimal> CalcularPrecioMenuAsync(int menuId);
        Task<bool> VerificarDisponibilidadMenuAsync(int menuId);
    }
}
