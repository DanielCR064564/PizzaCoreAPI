using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PizzaCoreAPI.Models
{
    public class Menu
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public decimal PrecioTotal { get; set; }

        public bool Activo { get; set; } = true;

        
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
        public virtual ICollection<Pizza> Pizzas { get; set; } = new List<Pizza>(); 
    }
}
