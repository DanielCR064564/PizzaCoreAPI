using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Models;

namespace PizzaCoreAPI.Interfaces
{
    public interface IVentaService
    {
        Task<IEnumerable<VentaDTO>> GetAllVentasAsync();
        Task<VentaDTO?> GetVentaByIdAsync(Guid id);
        Task<VentaDTO?> GetVentaByNumeroOrdenAsync(string numeroOrden);
        Task<IEnumerable<VentaDTO>> GetVentasByClienteAsync(string clienteId);
        Task<IEnumerable<VentaDTO>> GetVentasByEstadoAsync(EstadoVenta estado);
        Task<IEnumerable<VentaDTO>> GetVentasByFechaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<VentaDTO> CreateVentaAsync(CrearVentaDTO dto);
        Task<bool> UpdateVentaAsync(Guid id, CrearVentaDTO dto);
        Task<bool> DeleteVentaAsync(Guid id);
        Task<bool> CancelarVentaAsync(Guid id);
        Task<VentaDTO> CreateVentaFromSyncAsync(SyncVentaDTO syncDto);
    }
}
