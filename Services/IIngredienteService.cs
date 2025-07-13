using PizzaCoreAPI.DTOs;


namespace PizzaCoreAPI.Interfaces
{
    public interface IIngredienteService
    {
        Task<List<IngredienteDTO>> GetAllIngredientesAsync();
        Task<IngredienteDTO> GetIngredienteByIdAsync(Guid id);
        Task<IngredienteDTO> CreateIngredienteAsync(CrearIngredienteDTO ingredienteDto);
        Task UpdateIngredienteAsync(Guid id, CrearIngredienteDTO ingredienteDto);
        Task DeleteIngredienteAsync(Guid id);
    }
}