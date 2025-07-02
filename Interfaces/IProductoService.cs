using System.Collections.Generic;
using System.Threading.Tasks;
using PizzaCoreAPI.Models;
using PizzaCoreAPI.DTOs;

namespace PizzaCoreAPI.Interfaces
{
    public interface IProductoService
    {
        Task<List<ProductoDTO>> GetAllProductosAsync();
        Task<ProductoDTO> GetProductoByIdAsync(int id);
        Task<ProductoDTO> CreateProductoAsync(CrearProductoDTO productoDto);
        Task UpdateProductoAsync(int id, CrearProductoDTO productoDto);
        Task DeleteProductoAsync(int id);
        Task<List<ProductoDTO>> GetProductosDisponiblesAsync();
        Task<decimal> CalcularPrecioConIngredientesAsync(int productoId, List<int> ingredientesIds);
        Task<bool> VerificarDisponibilidadAsync(int productoId);
    }
}
