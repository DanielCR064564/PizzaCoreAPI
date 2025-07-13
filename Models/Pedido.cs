using PizzaCoreAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Pedido
{
    public string Id { get; set; } = string.Empty;

    [Required]
    public string ClienteId { get; set; } = string.Empty;

    public string? EmpleadoId { get; set; } // ✅ CAMBIADO de string? a Guid?

    [Required]
    public DateTime FechaPedido { get; set; } = DateTime.Now;

    public DateTime? FechaEntrega { get; set; }

    public string? Estado { get; set; } = "Pendiente";

    public decimal Total { get; set; } = 0;

    public string? MetodoPago { get; set; } = "Efectivo";

    public string? Notas { get; set; } = string.Empty;

    [JsonIgnore]
    public virtual Usuario? Cliente { get; set; }

    [JsonIgnore]
    public virtual Usuario? Empleado { get; set; }

    public virtual ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
