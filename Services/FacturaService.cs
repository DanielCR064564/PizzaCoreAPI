using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using PizzaCoreAPI.Data;
using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Models;

namespace PizzaCoreAPI.Services
{
    public class FacturaService : IFacturaService
    {
        private readonly PizzaDbContext _context;

        public FacturaService(PizzaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FacturaDTO>> GetAllFacturasAsync()
        {
            // Use direct SQL to completely bypass EF Core model mapping
            var facturas = new List<FacturaDTO>();

            using var connection = new MySqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync();

            using var command = new MySqlCommand(@"
                SELECT idfactura, fechaFactura, fkcliente, tipo_entrega, subtotal, iva, total, estado, empleadoId, fechaCreacion, notas
                FROM factura
                ORDER BY fechaFactura DESC", connection);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                facturas.Add(new FacturaDTO
                {
                    Id = reader.GetInt32("idfactura"),
                    FechaFactura = reader.GetDateTime("fechaFactura"),
                    FkCliente = reader.GetInt32("fkcliente"),
                    TipoEntrega = reader.GetString("tipo_entrega"),
                    Subtotal = reader.GetDecimal("subtotal"),
                    IVA = reader.GetDecimal("iva"),
                    Total = reader.GetDecimal("total"),
                    Estado = reader.GetString("estado"),
                    EmpleadoId = reader.IsDBNull(reader.GetOrdinal("empleadoId")) ? null : reader.GetInt32("empleadoId"),
                    FechaCreacion = reader.GetDateTime("fechaCreacion"),
                    Notas = reader.IsDBNull(reader.GetOrdinal("notas")) ? null : reader.GetString("notas"),
                    NombreCliente = "Cliente General",
                    DireccionCliente = "N/A",
                    NombreEmpleado = "Sistema",
                    RNC = "N/A",
                    RNCEmpleado = "N/A"
                });
            }

            return facturas;
        }

        public async Task<FacturaDTO?> GetFacturaByIdAsync(int id)
        {
            using var connection = new MySqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync();

            using var command = new MySqlCommand(@"
                SELECT idfactura, fechaFactura, fkcliente, tipo_entrega, subtotal, iva, total, estado, empleadoId, fechaCreacion, notas
                FROM factura
                WHERE idfactura = @id", connection);

            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new FacturaDTO
            {
                Id = reader.GetInt32("idfactura"),
                FechaFactura = reader.GetDateTime("fechaFactura"),
                FkCliente = reader.GetInt32("fkcliente"),
                TipoEntrega = reader.GetString("tipo_entrega"),
                Subtotal = reader.GetDecimal("subtotal"),
                IVA = reader.GetDecimal("iva"),
                Total = reader.GetDecimal("total"),
                Estado = reader.GetString("estado"),
                EmpleadoId = reader.IsDBNull(reader.GetOrdinal("empleadoId")) ? null : reader.GetInt32("empleadoId"),
                FechaCreacion = reader.GetDateTime("fechaCreacion"),
                Notas = reader.IsDBNull(reader.GetOrdinal("notas")) ? null : reader.GetString("notas"),
                NombreCliente = "Cliente General",
                DireccionCliente = "N/A",
                NombreEmpleado = "Sistema",
                RNC = "N/A",
                RNCEmpleado = "N/A"
            };
        }

        public async Task<FacturaDTO?> GetFacturaByPedidoIdAsync(string pedidoId)
        {
            // For now, return null since the new structure doesn't have pedidoId
            return await Task.FromResult<FacturaDTO?>(null);
        }

        public async Task<IEnumerable<FacturaDTO>> GetFacturasByEstadoAsync(EstadoFactura estado)
        {
            string estadoString = estado switch
            {
                EstadoFactura.Pendiente => "Pendiente",
                EstadoFactura.Pagado => "Pagado",
                EstadoFactura.Anulado => "Cancelado",
                _ => "Pendiente"
            };

            var facturas = await _context.Facturas
                .Where(f => f.Estado == estadoString)
                .OrderByDescending(f => f.FechaFactura)
                .ToListAsync();

            return facturas.Select(f => new FacturaDTO
            {
                Id = f.Id,
                FechaFactura = f.FechaFactura,
                FkCliente = f.FkCliente,
                TipoEntrega = f.TipoEntrega,
                Subtotal = f.Subtotal,
                IVA = f.IVA,
                Total = f.Total,
                Estado = f.Estado,
                EmpleadoId = f.EmpleadoId,
                FechaCreacion = f.FechaCreacion,
                Notas = f.Notas,
                NombreCliente = "Cliente General",
                DireccionCliente = "N/A",
                NombreEmpleado = "Sistema",
                RNC = "N/A",
                RNCEmpleado = "N/A"
            });
        }

        public async Task<IEnumerable<FacturaDTO>> GetFacturasByFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var facturas = await _context.Facturas
                .Where(f => f.FechaFactura >= fechaInicio && f.FechaFactura <= fechaFin)
                .OrderByDescending(f => f.FechaFactura)
                .ToListAsync();

            return facturas.Select(f => new FacturaDTO
            {
                Id = f.Id,
                FechaFactura = f.FechaFactura,
                FkCliente = f.FkCliente,
                TipoEntrega = f.TipoEntrega,
                Subtotal = f.Subtotal,
                IVA = f.IVA,
                Total = f.Total,
                Estado = f.Estado,
                EmpleadoId = f.EmpleadoId,
                FechaCreacion = f.FechaCreacion,
                Notas = f.Notas,
                NombreCliente = "Cliente General",
                DireccionCliente = "N/A",
                NombreEmpleado = "Sistema",
                RNC = "N/A",
                RNCEmpleado = "N/A"
            });
        }

        public async Task<FacturaDTO> CreateFacturaFromPedidoAsync(string pedidoId)
        {
            await Task.CompletedTask;
            throw new NotImplementedException("CreateFacturaFromPedidoAsync not implemented for current database structure");
        }

        public async Task<bool> UpdateEstadoFacturaAsync(int id, EstadoFactura estado)
        {
            var factura = await _context.Facturas.FindAsync(id);
            if (factura == null) return false;

            string estadoString = estado switch
            {
                EstadoFactura.Pendiente => "Pendiente",
                EstadoFactura.Pagado => "Pagado",
                EstadoFactura.Anulado => "Cancelado",
                _ => "Pendiente"
            };

            factura.Estado = estadoString;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<byte[]> GenerarFacturaPDFAsync(int facturaId)
        {
            var factura = await GetFacturaByIdAsync(facturaId);
            if (factura == null)
                throw new InvalidOperationException("Factura no encontrada");

            var pdfContent = System.Text.Encoding.UTF8.GetBytes($"Factura #{factura.NumeroFactura} - Total: ${factura.Total}");
            return pdfContent;
        }

        public async Task<string> GenerarNumeroFacturaAsync()
        {
            var ultimaFactura = await _context.Facturas
                .OrderByDescending(f => f.Id)
                .FirstOrDefaultAsync();

            int siguienteNumero = ultimaFactura?.Id + 1 ?? 1;
            return $"F-{siguienteNumero:D6}";
        }

        public async Task<IEnumerable<FacturaDTO>> GetFacturasByClienteAsync(string clienteId)
        {
            if (!int.TryParse(clienteId, out int clienteIdInt))
            {
                return Enumerable.Empty<FacturaDTO>();
            }

            var facturas = await _context.Facturas
                .Where(f => f.FkCliente == clienteIdInt)
                .OrderByDescending(f => f.FechaFactura)
                .ToListAsync();

            return facturas.Select(f => new FacturaDTO
            {
                Id = f.Id,
                FechaFactura = f.FechaFactura,
                FkCliente = f.FkCliente,
                TipoEntrega = f.TipoEntrega,
                Subtotal = f.Subtotal,
                IVA = f.IVA,
                Total = f.Total,
                Estado = f.Estado,
                EmpleadoId = f.EmpleadoId,
                FechaCreacion = f.FechaCreacion,
                Notas = f.Notas,
                NombreCliente = "Cliente General",
                DireccionCliente = "N/A",
                NombreEmpleado = "Sistema",
                RNC = "N/A",
                RNCEmpleado = "N/A"
            });
        }

        public async Task<bool> PuedeCrearFacturaAsync(string pedidoId)
        {
            // Since the current database structure doesn't have pedidoId linked to facturas,
            // this method returns false for now
            return await Task.FromResult(false);
        }

        public async Task<EstadisticasFacturaDTO> GetEstadisticasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var facturas = await _context.Facturas
                .Where(f => f.FechaFactura >= fechaInicio && f.FechaFactura <= fechaFin)
                .ToListAsync();

            var totalFacturas = facturas.Count;
            var totalMonto = facturas.Sum(f => f.Total);
            var totalIVA = facturas.Sum(f => f.IVA);
            var promedioFactura = totalFacturas > 0 ? totalMonto / totalFacturas : 0;

            var facturasPendientes = facturas.Count(f => f.Estado == "Pendiente");
            var facturasPagadas = facturas.Count(f => f.Estado == "Pagado");
            var facturasAnuladas = facturas.Count(f => f.Estado == "Cancelado");
            var facturasVencidas = facturas.Count(f => f.Estado == "Vencido");

            var dias = (fechaFin - fechaInicio).TotalDays;
            var montoPromedioDiario = dias > 0 ? totalMonto / (decimal)dias : 0;
            var tasaCobranza = totalFacturas > 0 ? (facturasPagadas / (decimal)totalFacturas) * 100 : 0;

            return new EstadisticasFacturaDTO
            {
                TotalFacturas = totalFacturas,
                TotalMonto = totalMonto,
                TotalIVA = totalIVA,
                PromedioFactura = promedioFactura,
                FacturasPendientes = facturasPendientes,
                FacturasPagadas = facturasPagadas,
                FacturasAnuladas = facturasAnuladas,
                FacturasVencidas = facturasVencidas,
                PeriodoInicio = fechaInicio,
                PeriodoFin = fechaFin,
                MontoPromedioDiario = montoPromedioDiario,
                TasaCobranza = tasaCobranza
            };
        }
    }
}
