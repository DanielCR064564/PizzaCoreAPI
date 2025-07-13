using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        public List<IngredienteDTO> Ingredientes { get; set; } = new();
    }

    public class CrearProductoDTO
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public string Tipo { get; set; } // ✅ Se cambió de enum a string

        public bool Disponible { get; set; } = true;

        [Required]
        public List<Guid> IngredientesIds { get; set; }
    }
}
