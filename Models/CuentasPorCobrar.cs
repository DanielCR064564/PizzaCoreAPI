using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaCoreAPI.Models
{
    public class CuentasPorCobrar
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string ClienteId { get; set; } = string.Empty;
        public string? PedidoId { get; set; }
        [Required]
        public decimal MontoPendiente { get; set; }
        [Required]
        public DateTime FechaVencimiento { get; set; }
        [Required]
        public string Estado { get; set; } = "Pendiente";
    }
}
