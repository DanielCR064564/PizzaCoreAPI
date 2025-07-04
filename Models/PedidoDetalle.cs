using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PizzaCoreAPI.Models
{
    public class PedidoDetalle
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        public string PedidoId { get; set; } = string.Empty;

        [Required]
        public Guid ProductoId { get; set; }

        public string Cantidad { get; set; } = "1";
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }

        [JsonIgnore]
        public virtual Pedido Pedido { get; set; } = null!;
        public virtual Producto Producto { get; set; } = null!;
    }
}
