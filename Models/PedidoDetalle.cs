using PizzaCoreAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class PedidoDetalle
{
    public string Id { get; set; } = string.Empty;

    [Required]
    public string PedidoId { get; set; } = string.Empty;

    [Required]
    public Guid ProductoId { get; set; }

    public int Cantidad { get; set; } = 1; // ✅ CAMBIADO de string a int

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    [JsonIgnore]
    public virtual Pedido Pedido { get; set; } = null!;

    public virtual Producto Producto { get; set; } = null!;
}
