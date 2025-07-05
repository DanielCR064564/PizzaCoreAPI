using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PizzaCoreAPI.Models
{
    public class Ingrediente
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public decimal PrecioAdicional { get; set; } = 0;
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public TipoIngrediente Tipo { get; set; } = TipoIngrediente.Vegetal;

        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
