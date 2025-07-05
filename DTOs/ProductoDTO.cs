using System;
using System.Collections.Generic;

namespace PizzaCoreAPI.DTOs
{
    public class ProductoDTO
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string Tipo { get; set; }
        public bool Disponible { get; set; }
        public List<IngredienteDTO> Ingredientes { get; set; } = new List<IngredienteDTO>();
    }

    public class CrearProductoDTO
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string Tipo { get; set; }
        public bool Disponible { get; set; }
        public List<Guid> IngredientesIds { get; set; }
    }
}
