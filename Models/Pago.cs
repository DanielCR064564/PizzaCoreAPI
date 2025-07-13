using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaCoreAPI.Models
{
    public class Pago
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public decimal Monto { get; set; }
        [Required]
        public DateTime FechaPago { get; set; } = DateTime.Now;
        [Required]
        public Guid MetodoDePagoId { get; set; }
        public string PedidoId { get; set; } = string.Empty;
        public string ClienteId { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }
}
