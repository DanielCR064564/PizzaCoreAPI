using System.Collections.Generic;
using System.Threading.Tasks;
using PizzaCoreAPI.Models;
using PizzaCoreAPI.DTOs;

namespace PizzaCoreAPI.Interfaces
{
    public interface IMenuService
    {
        Task<List<MenuDTO>> GetAllMenusAsync();
        Task<MenuDTO> GetMenuByIdAsync(Guid id);
        Task<MenuDTO> CreateMenuAsync(CrearMenuDTO menuDto);
        Task UpdateMenuAsync(Guid id, CrearMenuDTO menuDto);
        Task DeleteMenuAsync(Guid id);
        Task<List<MenuDTO>> GetMenusActivosAsync();
        Task<decimal> CalcularPrecioMenuAsync(Guid menuId);
        Task<bool> VerificarDisponibilidadMenuAsync(Guid menuId);
    }
}
