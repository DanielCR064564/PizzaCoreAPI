using System;

namespace PizzaCoreAPI.DTOs
{
    public class CuentasPorCobrarDTO
    {
        public string Id { get; set; } = string.Empty;
        public string ClienteId { get; set; } = string.Empty;
        public string? PedidoId { get; set; }
        public decimal MontoPendiente { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Estado { get; set; } = "Pendiente";
    }

    public class CrearCuentasPorCobrarDTO
    {
        public string ClienteId { get; set; } = string.Empty;
        public string? PedidoId { get; set; }
        public decimal MontoPendiente { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Estado { get; set; } = "Pendiente";
    }
}
