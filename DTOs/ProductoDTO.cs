using System.Collections.Generic;

namespace PizzaCoreAPI.DTOs
{
    public class ProductoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string Tipo { get; set; }
        public bool Disponible { get; set; }
        public string ImagenUrl { get; set; }
        public List<IngredienteDTO> Ingredientes { get; set; }
    }

    public class CrearProductoDTO
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string Tipo { get; set; }
        public bool Disponible { get; set; }
        public string ImagenUrl { get; set; }
        public List<int> IngredientesIds { get; set; }
    }
}
