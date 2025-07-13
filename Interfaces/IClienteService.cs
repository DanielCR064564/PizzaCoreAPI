// Services/IClienteService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using PizzaCoreAPI.DTOs;

namespace PizzaCoreAPI.Interfaces
{
    public interface IClienteService
    {
        Task<List<ClienteDTO>> GetAllClientesAsync();
        Task<ClienteDTO?> GetClienteByIdAsync(string id);
        Task<ClienteDTO> CreateClienteAsync(CrearClienteDTO clienteDto);
        Task<bool> UpdateClienteAsync(string id, ActualizarClienteDTO clienteDto);
        Task<bool> DeleteClienteAsync(string id);
        Task<bool> ClienteExistsAsync(string id);
        Task<bool> UserNameExistsAsync(string userName);
        Task<bool> EmailExistsAsync(string email);
    }
}

