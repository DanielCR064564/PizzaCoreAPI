using PizzaCoreAPI.Models;
using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace PizzaCoreAPI.Services
{
    public class MetodoDePagoService : IMetodoDePagoService
    {
        private readonly PizzaDbContext _context;
        public MetodoDePagoService(PizzaDbContext context) { _context = context; }
        public async Task<IEnumerable<MetodoDePagoDTO>> GetAllAsync() => await _context.MetodosDePago.Select(m => new MetodoDePagoDTO {
            Id = m.Id,
            Nombre = m.Nombre,
            Descripcion = m.Descripcion
        }).ToListAsync();
        public async Task<MetodoDePagoDTO?> GetByIdAsync(Guid id) => await _context.MetodosDePago.Where(m => m.Id == id).Select(m => new MetodoDePagoDTO {
            Id = m.Id,
            Nombre = m.Nombre,
            Descripcion = m.Descripcion
        }).FirstOrDefaultAsync();
        public async Task<MetodoDePagoDTO> CreateAsync(CrearMetodoDePagoDTO dto)
        {
            var metodo = new MetodoDePago {
                Id = Guid.NewGuid(),
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion
            };
            _context.MetodosDePago.Add(metodo);
            await _context.SaveChangesAsync();
            return new MetodoDePagoDTO {
                Id = metodo.Id,
                Nombre = metodo.Nombre,
                Descripcion = metodo.Descripcion
            };
        }
        public async Task<bool> UpdateAsync(Guid id, CrearMetodoDePagoDTO dto)
        {
            var metodo = await _context.MetodosDePago.FindAsync(id);
            if (metodo == null) return false;
            metodo.Nombre = dto.Nombre;
            metodo.Descripcion = dto.Descripcion;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var metodo = await _context.MetodosDePago.FindAsync(id);
            if (metodo == null) return false;
            _context.MetodosDePago.Remove(metodo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
