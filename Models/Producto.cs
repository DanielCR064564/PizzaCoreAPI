using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PizzaCoreAPI.Models
{
    public enum TipoProducto
    {
        Pizza,
        Bebida,
        Postre,
        Entrada,
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

        public int Stock { get; set; } = 0;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        public string? ApiId { get; set; }

        public bool SincronizadoConAPI { get; set; } = false;

        public string? Imagen { get; set; }

        public string? IngredientesTexto { get; set; }

        public int TiempoPreparacion { get; set; } = 15;

        public virtual ICollection<Ingrediente> Ingredientes { get; set; } = new List<Ingrediente>();
        public virtual ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
        public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();
    }
}
