using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaCoreAPI.DTOs
{
    public class IngredienteDTO
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioAdicional { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty; 
    }

    public class CrearIngredienteDTO
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        
        public decimal PrecioAdicional { get; set; } = 0;
        
        public string Descripcion { get; set; } = string.Empty;
        
        [Required]
        public string Tipo { get; set; } = "Vegetal";
    }
}