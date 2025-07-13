using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PizzaCoreAPI.Models
{
    public class Pizza
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public decimal Precio { get; set; }



        public bool Disponible { get; set; } = true;

        public virtual ICollection<Ingrediente> Ingredientes { get; set; } = new List<Ingrediente>();
        public virtual ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();

        public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>(); 
    }
}
