using Microsoft.EntityFrameworkCore;
using PizzaCoreAPI.Data;
using PizzaCoreAPI.Models;

namespace PizzaCoreAPI
{
    public class TestFactura
    {
        public static async Task TestFacturaQuery()
        {
            var options = new DbContextOptionsBuilder<PizzaDbContext>()
                .UseMySql("Server=localhost;Database=pizzacoredb;Uid=root;Pwd=root;",
                    ServerVersion.AutoDetect("Server=localhost;Database=pizzacoredb;Uid=root;Pwd=root;"))
                .Options;

            using var context = new PizzaDbContext(options);

            // Try to query facturas directly without any relationships
            var facturas = await context.Facturas
                .Select(f => new
                {
                    f.Id,
                    f.FechaFactura,
                    f.FkCliente,
                    f.TipoEntrega,
                    f.Subtotal,
                    f.IVA,
                    f.Total,
                    f.Estado,
                    f.EmpleadoId,
                    f.FechaCreacion,
                    f.Notas
                })
                .ToListAsync();

            Console.WriteLine($"Found {facturas.Count} facturas");
            foreach (var factura in facturas)
            {
                Console.WriteLine($"Factura ID: {factura.Id}, Cliente: {factura.FkCliente}, Total: {factura.Total}");
            }
        }
    }
}
