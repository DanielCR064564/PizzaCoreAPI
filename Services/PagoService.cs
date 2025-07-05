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
    public class PagoService : IPagoService
    {
        private readonly PizzaDbContext _context;
        public PagoService(PizzaDbContext context) { _context = context; }
        public async Task<IEnumerable<PagoDTO>> GetAllAsync() => await _context.Pagos.Select(p => new PagoDTO {
            Id = p.Id,
            Monto = p.Monto,
            FechaPago = p.FechaPago,
            MetodoDePagoId = p.MetodoDePagoId,
            PedidoId = p.PedidoId,
            ClienteId = p.ClienteId,
            Observaciones = p.Observaciones
        }).ToListAsync();
        public async Task<PagoDTO?> GetByIdAsync(string id) => await _context.Pagos.Where(p => p.Id == id).Select(p => new PagoDTO {
            Id = p.Id,
            Monto = p.Monto,
            FechaPago = p.FechaPago,
            MetodoDePagoId = p.MetodoDePagoId,
            PedidoId = p.PedidoId,
            ClienteId = p.ClienteId,
            Observaciones = p.Observaciones
        }).FirstOrDefaultAsync();
        public async Task<PagoDTO> CreateAsync(CrearPagoDTO dto)
        {
            var pago = new Pago {
                Id = Guid.NewGuid().ToString(),
                Monto = dto.Monto,
                FechaPago = DateTime.Now,
                MetodoDePagoId = dto.MetodoDePagoId,
                PedidoId = dto.PedidoId,
                ClienteId = dto.ClienteId,
                Observaciones = dto.Observaciones
            };
            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();
            return new PagoDTO {
                Id = pago.Id,
                Monto = pago.Monto,
                FechaPago = pago.FechaPago,
                MetodoDePagoId = pago.MetodoDePagoId,
                PedidoId = pago.PedidoId,
                ClienteId = pago.ClienteId,
                Observaciones = pago.Observaciones
            };
        }
        public async Task<bool> UpdateAsync(string id, CrearPagoDTO dto)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null) return false;
            pago.Monto = dto.Monto;
            pago.MetodoDePagoId = dto.MetodoDePagoId;
            pago.PedidoId = dto.PedidoId;
            pago.ClienteId = dto.ClienteId;
            pago.Observaciones = dto.Observaciones;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(string id)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null) return false;
            _context.Pagos.Remove(pago);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
