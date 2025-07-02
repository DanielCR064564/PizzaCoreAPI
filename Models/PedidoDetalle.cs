using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PizzaCoreAPI.Models
{
    public class PedidoDetalle
    {
        public int Id { get; set; }

        [Required]
        public int PedidoId { get; set; }

        [Required]
        public int ProductoId { get; set; }

        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }

        [JsonIgnore]
        public virtual Pedido Pedido { get; set; } = null!;
        public virtual Producto Producto { get; set; } = null!;
    }
}
