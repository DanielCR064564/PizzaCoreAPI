using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Models;

namespace PizzaCoreAPI.Interfaces
{
    public interface IVentaService
    {
        Task<IEnumerable<VentaDTO>> GetAllVentasAsync();
        Task<VentaDTO?> GetVentaByIdAsync(string id);
        Task<VentaDTO?> GetVentaByNumeroOrdenAsync(string numeroOrden);
        Task<IEnumerable<VentaDTO>> GetVentasByClienteAsync(string clienteId);
        Task<IEnumerable<VentaDTO>> GetVentasByEstadoAsync(EstadoVenta estado);
        Task<IEnumerable<VentaDTO>> GetVentasByFechaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<VentaDTO> CreateVentaAsync(CrearVentaDTO dto, bool validateClients = true);
        Task<bool> UpdateVentaAsync(string id, CrearVentaDTO dto);
        Task<bool> DeleteVentaAsync(string id);
        Task<bool> CancelarVentaAsync(string id);
        Task<VentaDTO> CreateVentaFromSyncAsync(SyncVentaDTO syncDto);
    }
}