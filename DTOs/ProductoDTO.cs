using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PizzaCoreAPI.DTOs
{
    public class ProductoDTO
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public bool Disponible { get; set; }
        public int Stock { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public string? ApiId { get; set; }
        public bool SincronizadoConAPI { get; set; }
        public string? Imagen { get; set; }
        public string? IngredientesTexto { get; set; }
        public int TiempoPreparacion { get; set; }
        public List<IngredienteDTO> Ingredientes { get; set; } = new();
    }

    public class CrearProductoDTO
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public string Tipo { get; set; } = string.Empty;

        public bool Disponible { get; set; } = true;

        public int Stock { get; set; } = 0;

        public string? ApiId { get; set; }

        public bool SincronizadoConAPI { get; set; } = false;

        public string? Imagen { get; set; }

        public string? IngredientesTexto { get; set; }

        public int TiempoPreparacion { get; set; } = 15;

        [Required]
        public List<Guid> IngredientesIds { get; set; } = new();
    }
}
