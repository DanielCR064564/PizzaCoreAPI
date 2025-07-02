using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PizzaCoreAPI.Models
{
    public class Ingrediente
    {
        public int Id { get; set; }
        
        [Required]
        public string Nombre { get; set; } = string.Empty;
        
        public decimal PrecioAdicional { get; set; } = 0;
        public string Descripcion { get; set; } = string.Empty;
        public string Tipo { get; set; } = "Vegetal"; // Carne, Vegetal, Queso, etc.
        
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
