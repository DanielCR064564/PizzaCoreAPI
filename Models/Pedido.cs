using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PizzaCoreAPI.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        [Required]
        public string ClienteId { get; set; } = string.Empty;

        [Required]
        public string EmpleadoId { get; set; } = string.Empty;

        [Required]
        public DateTime FechaPedido { get; set; } = DateTime.Now;

        public DateTime? FechaEntrega { get; set; }

        public string? Estado { get; set; } = "Pendiente"; // Pendiente, EnPreparacion, Listo, Entregado, Cancelado

        public decimal Total { get; set; } = 0;

        public string? MetodoPago { get; set; } = "Efectivo"; // Efectivo, Tarjeta, Transferencia

        public string? Notas { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual Usuario? Cliente { get; set; }

        [JsonIgnore]
        public virtual Usuario? Empleado { get; set; }

        public virtual ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
