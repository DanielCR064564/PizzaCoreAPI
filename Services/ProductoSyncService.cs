using System.Data;
using MySqlConnector;
using PizzaCoreAPI.Controllers;

namespace PizzaCoreAPI.Services
{
    public class ProductoSyncService
    {
        private readonly string _connectionString;

        public ProductoSyncService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PizzaDb") ?? throw new ArgumentNullException("Connection string not found");
        }

        public async Task<List<ProductoSyncDTO>> GetAllProductosForSyncAsync()
        {
            var productos = new List<ProductoSyncDTO>();

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"SELECT 
                            apiId,
                            nombre,
                            descripcion,
                            precio,
                            tipo,
                            disponible,
                            fechaActualizacion
                          FROM productos 
                          WHERE apiId IS NOT NULL AND apiId != ''";

            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                productos.Add(new ProductoSyncDTO
                {
                    Id = reader.GetString("apiId"),
                    Nombre = reader.GetString("nombre"),
                    Descripcion = reader.GetString("descripcion"),
                    Precio = reader.GetDecimal("precio"),
                    Tipo = reader.GetString("tipo"),
                    Disponible = reader.GetBoolean("disponible"),
                    FechaActualizacion = reader.GetDateTime("fechaActualizacion")
                });
            }

            return productos;
        }
    }
}
