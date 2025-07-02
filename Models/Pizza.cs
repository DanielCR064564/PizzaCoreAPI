using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PizzaCoreAPI.Models
{
    public class Pizza
    {
        public int Id { get; set; }
        
        [Required]
        public string Nombre { get; set; } = string.Empty;
        
        [Required]
        public string Descripcion { get; set; } = string.Empty;
        
        [Required]
        public decimal Precio { get; set; }
        
        public string ImagenUrl { get; set; } = string.Empty;
        
        public bool Disponible { get; set; } = true;
        
        public virtual ICollection<Ingrediente> Ingredientes { get; set; } = new List<Ingrediente>();
        
        public virtual ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
    }
}
