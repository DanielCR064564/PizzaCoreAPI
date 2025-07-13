// Services/ClienteService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PizzaCoreAPI.Data;
using PizzaCoreAPI.Models;
using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Interfaces;

namespace PizzaCoreAPI.Services
{
    public class ClienteService : IClienteService
    {
        private readonly PizzaDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public ClienteService(PizzaDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<ClienteDTO>> GetAllClientesAsync()
        {
            var clientes = await _context.Usuarios
                .Where(u => !u.EsEmpleado)
                .Include(u => u.Pedidos)
                .ToListAsync();

            return clientes.Select(MapToDTO).ToList();
        }

        public async Task<ClienteDTO?> GetClienteByIdAsync(string id)
        {
            var cliente = await _context.Usuarios
                .Where(u => !u.EsEmpleado && u.Id == id)
                .Include(u => u.Pedidos)
                .FirstOrDefaultAsync();

            return cliente != null ? MapToDTO(cliente) : null;
        }

        public async Task<ClienteDTO> CreateClienteAsync(CrearClienteDTO clienteDto)
        {
            var usuario = new Usuario
            {
                UserName = clienteDto.UserName,
                Email = clienteDto.Email,
                NombreCompleto = clienteDto.NombreCompleto,
                PhoneNumber = clienteDto.PhoneNumber,
                Direccion = clienteDto.Direccion,
                RNC = clienteDto.RNC,
                EsEmpleado = false, 
                EmailConfirmed = true 
            };

            var result = await _userManager.CreateAsync(usuario, clienteDto.Password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Error creating client: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // Add to Cliente role
            await _userManager.AddToRoleAsync(usuario, "Cliente");

            return MapToDTO(usuario);
        }

        public async Task<bool> UpdateClienteAsync(string id, ActualizarClienteDTO clienteDto)
        {
            var cliente = await _context.Usuarios
                .Where(u => !u.EsEmpleado && u.Id == id)
                .FirstOrDefaultAsync();

            if (cliente == null)
                return false;

            // Update fields
            cliente.NombreCompleto = clienteDto.NombreCompleto;
            cliente.UserName = clienteDto.UserName;
            cliente.Email = clienteDto.Email;
            cliente.PhoneNumber = clienteDto.PhoneNumber;
            cliente.Direccion = clienteDto.Direccion;
            cliente.RNC = clienteDto.RNC;

            var result = await _userManager.UpdateAsync(cliente);
            return result.Succeeded;
        }

        public async Task<bool> DeleteClienteAsync(string id)
        {
            var cliente = await _context.Usuarios
                .Where(u => !u.EsEmpleado && u.Id == id)
                .FirstOrDefaultAsync();

            if (cliente == null)
                return false;

            var result = await _userManager.DeleteAsync(cliente);
            return result.Succeeded;
        }

        public async Task<bool> ClienteExistsAsync(string id)
        {
            return await _context.Usuarios
                .AnyAsync(u => !u.EsEmpleado && u.Id == id);
        }

        public async Task<bool> UserNameExistsAsync(string userName)
        {
            return await _context.Usuarios
                .AnyAsync(u => u.UserName == userName);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Usuarios
                .AnyAsync(u => u.Email == email);
        }

        private ClienteDTO MapToDTO(Usuario usuario)
        {
            return new ClienteDTO
            {
                Id = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                UserName = usuario.UserName ?? string.Empty,
                Email = usuario.Email ?? string.Empty,
                PhoneNumber = usuario.PhoneNumber ?? string.Empty,
                Direccion = usuario.Direccion,
                RNC = usuario.RNC,
                TotalPedidos = usuario.Pedidos?.Count ?? 0
            };
        }
    }
}