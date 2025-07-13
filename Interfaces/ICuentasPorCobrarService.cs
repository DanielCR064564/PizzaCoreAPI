using PizzaCoreAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaCoreAPI.Services
{
    public interface ICuentasPorCobrarService
    {
        Task<IEnumerable<CuentasPorCobrarDTO>> GetAllAsync();
        Task<CuentasPorCobrarDTO?> GetByIdAsync(string id);
        Task<CuentasPorCobrarDTO> CreateAsync(CrearCuentasPorCobrarDTO dto);
        Task<bool> UpdateAsync(string id, CrearCuentasPorCobrarDTO dto);
        Task<bool> DeleteAsync(string id);
    }
}
