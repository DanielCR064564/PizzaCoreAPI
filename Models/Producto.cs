using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PizzaCoreAPI.Models
{
    public enum TipoProducto
    {
        Pizza,
        Bebida,
        Complemento
    }

    public class Producto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public TipoProducto Tipo { get; set; }

        public bool Disponible { get; set; } = true;

        public string ImagenUrl { get; set; } = string.Empty;

        public virtual ICollection<Ingrediente> Ingredientes { get; set; } = new List<Ingrediente>();
        public virtual ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
        public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();
    }
}
