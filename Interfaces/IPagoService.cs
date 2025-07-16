using PizzaCoreAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaCoreAPI.Services
{
    public interface IPagoService
    {
        Task<IEnumerable<PagoDTO>> GetAllAsync();
        Task<PagoDTO?> GetByIdAsync(string id);
        Task<PagoDTO> CreateAsync(CrearPagoDTO dto);
        Task<bool> UpdateAsync(string id, CrearPagoDTO dto);
        Task<bool> DeleteAsync(string id);
    }
}
