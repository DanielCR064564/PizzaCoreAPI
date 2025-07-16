using Microsoft.EntityFrameworkCore;
using PizzaCoreAPI.Data;
using PizzaCoreAPI.Models;
using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Interfaces;

namespace PizzaCoreAPI.Services
{
    public class IngredienteService : IIngredienteService
    {
        private readonly PizzaDbContext _context;

        public IngredienteService(PizzaDbContext context)
        {
            _context = context;
        }

        public async Task<List<IngredienteDTO>> GetAllIngredientesAsync()
        {
            var ingredientes = await _context.Ingredientes.ToListAsync();

            return ingredientes.Select(i => new IngredienteDTO
            {
                Id = i.Id,
                Nombre = i.Nombre,
                PrecioAdicional = i.PrecioAdicional,
                Descripcion = i.Descripcion,
                Tipo = i.Tipo.ToString() // ✅ Convert enum to string
            }).ToList();
        }

        public async Task<IngredienteDTO> GetIngredienteByIdAsync(Guid id)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);
            if (ingrediente == null)
                return null;

            return new IngredienteDTO
            {
                Id = ingrediente.Id,
                Nombre = ingrediente.Nombre,
                PrecioAdicional = ingrediente.PrecioAdicional,
                Descripcion = ingrediente.Descripcion,
                Tipo = ingrediente.Tipo.ToString() // ✅ Convert enum to string
            };
        }

        public async Task<IngredienteDTO> CreateIngredienteAsync(CrearIngredienteDTO ingredienteDto)
        {
            var ingrediente = new Ingrediente
            {
                Nombre = ingredienteDto.Nombre,
                PrecioAdicional = ingredienteDto.PrecioAdicional,
                Descripcion = ingredienteDto.Descripcion,
                Tipo = Enum.Parse<TipoIngrediente>(ingredienteDto.Tipo, ignoreCase: true) // ✅ Convert string to enum
            };

            await _context.Ingredientes.AddAsync(ingrediente);
            await _context.SaveChangesAsync();

            return new IngredienteDTO
            {
                Id = ingrediente.Id,
                Nombre = ingrediente.Nombre,
                PrecioAdicional = ingrediente.PrecioAdicional,
                Descripcion = ingrediente.Descripcion,
                Tipo = ingrediente.Tipo.ToString()
            };
        }

        public async Task UpdateIngredienteAsync(Guid id, CrearIngredienteDTO ingredienteDto)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);
            if (ingrediente == null)
                return;

            ingrediente.Nombre = ingredienteDto.Nombre;
            ingrediente.PrecioAdicional = ingredienteDto.PrecioAdicional;
            ingrediente.Descripcion = ingredienteDto.Descripcion;
            ingrediente.Tipo = Enum.Parse<TipoIngrediente>(ingredienteDto.Tipo, ignoreCase: true); 

            await _context.SaveChangesAsync();
        }

        public async Task DeleteIngredienteAsync(Guid id)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);
            if (ingrediente != null)
            {
                _context.Ingredientes.Remove(ingrediente);
                await _context.SaveChangesAsync();
            }
        }
    }
}