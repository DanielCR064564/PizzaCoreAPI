using PizzaCoreAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaCoreAPI.Services
{
    public interface IMetodoDePagoService
    {
        Task<IEnumerable<MetodoDePagoDTO>> GetAllAsync();
        Task<MetodoDePagoDTO?> GetByIdAsync(Guid id);
        Task<MetodoDePagoDTO> CreateAsync(CrearMetodoDePagoDTO dto);
        Task<bool> UpdateAsync(Guid id, CrearMetodoDePagoDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
