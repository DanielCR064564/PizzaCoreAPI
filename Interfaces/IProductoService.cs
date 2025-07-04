using System.Collections.Generic;
using System.Threading.Tasks;
using PizzaCoreAPI.Models;
using PizzaCoreAPI.DTOs;

namespace PizzaCoreAPI.Interfaces
{
    public interface IProductoService
    {
        Task<List<ProductoDTO>> GetAllProductosAsync();
        Task<ProductoDTO> GetProductoByIdAsync(Guid id);
        Task<ProductoDTO> CreateProductoAsync(CrearProductoDTO productoDto);
        Task UpdateProductoAsync(Guid id, CrearProductoDTO productoDto);
        Task DeleteProductoAsync(Guid id);
        Task<List<ProductoDTO>> GetProductosDisponiblesAsync();
        Task<decimal> CalcularPrecioConIngredientesAsync(Guid productoId, List<Guid> ingredientesIds);
        Task<bool> VerificarDisponibilidadAsync(Guid productoId);
    }
}
