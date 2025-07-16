using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Models;

namespace PizzaCoreAPI.Interfaces
{
    public interface IPedidoService
    {
        Task<IEnumerable<PedidoDTO>> GetAllPedidosAsync();
        Task<PedidoDTO?> GetPedidoByIdAsync(string id);
        Task<PedidoDTO> CreatePedidoAsync(CrearPedidoDTO dto);
        Task<bool> UpdatePedidoAsync(string id, CrearPedidoDTO dto);
        Task<bool> DeletePedidoAsync(string id);
        Task<bool> UpdateEstadoPedidoAsync(string id, EstadoPedido estado);
        Task<decimal> CalcularTotalPedidoAsync(List<CrearPedidoDetalleDTO> detalles);
        Task<bool> ValidarDisponibilidadProductosAsync(List<CrearPedidoDetalleDTO> detalles);
        Task<IEnumerable<PedidoDTO>> GetPedidosByClienteAsync(string clienteId);
        Task<IEnumerable<PedidoDTO>> GetPedidosByEstadoAsync(EstadoPedido estado);
    }
}