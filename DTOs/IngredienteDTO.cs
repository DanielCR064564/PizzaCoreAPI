namespace PizzaCoreAPI.DTOs
{
    public class IngredienteDTO
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioAdicional { get; set; }
        public string Tipo { get; set; }
    }
}
