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
    public class CuentasPorCobrarService : ICuentasPorCobrarService
    {
        private readonly PizzaDbContext _context;
        public CuentasPorCobrarService(PizzaDbContext context) { _context = context; }
        public async Task<IEnumerable<CuentasPorCobrarDTO>> GetAllAsync() => await _context.CuentasPorCobrar.Select(c => new CuentasPorCobrarDTO {
            Id = c.Id,
            ClienteId = c.ClienteId,
            PedidoId = c.PedidoId,
            MontoPendiente = c.MontoPendiente,
            FechaVencimiento = c.FechaVencimiento,
            Estado = c.Estado
        }).ToListAsync();
        public async Task<CuentasPorCobrarDTO?> GetByIdAsync(string id) => await _context.CuentasPorCobrar.Where(c => c.Id == id).Select(c => new CuentasPorCobrarDTO {
            Id = c.Id,
            ClienteId = c.ClienteId,
            PedidoId = c.PedidoId,
            MontoPendiente = c.MontoPendiente,
            FechaVencimiento = c.FechaVencimiento,
            Estado = c.Estado
        }).FirstOrDefaultAsync();
        public async Task<CuentasPorCobrarDTO> CreateAsync(CrearCuentasPorCobrarDTO dto)
        {
            var cuenta = new CuentasPorCobrar {
                Id = Guid.NewGuid().ToString(),
                ClienteId = dto.ClienteId,
                PedidoId = dto.PedidoId,
                MontoPendiente = dto.MontoPendiente,
                FechaVencimiento = dto.FechaVencimiento,
                Estado = dto.Estado
            };
            _context.CuentasPorCobrar.Add(cuenta);
            await _context.SaveChangesAsync();
            return new CuentasPorCobrarDTO {
                Id = cuenta.Id,
                ClienteId = cuenta.ClienteId,
                PedidoId = cuenta.PedidoId,
                MontoPendiente = cuenta.MontoPendiente,
                FechaVencimiento = cuenta.FechaVencimiento,
                Estado = cuenta.Estado
            };
        }
        public async Task<bool> UpdateAsync(string id, CrearCuentasPorCobrarDTO dto)
        {
            var cuenta = await _context.CuentasPorCobrar.FindAsync(id);
            if (cuenta == null) return false;
            cuenta.ClienteId = dto.ClienteId;
            cuenta.PedidoId = dto.PedidoId;
            cuenta.MontoPendiente = dto.MontoPendiente;
            cuenta.FechaVencimiento = dto.FechaVencimiento;
            cuenta.Estado = dto.Estado;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(string id)
        {
            var cuenta = await _context.CuentasPorCobrar.FindAsync(id);
            if (cuenta == null) return false;
            _context.CuentasPorCobrar.Remove(cuenta);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
