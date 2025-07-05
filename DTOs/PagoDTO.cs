using System;

namespace PizzaCoreAPI.DTOs
{
    public class PagoDTO
    {
        public string Id { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public Guid MetodoDePagoId { get; set; }
        public string PedidoId { get; set; } = string.Empty;
        public string ClienteId { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }

    public class CrearPagoDTO
    {
        public decimal Monto { get; set; }
        public Guid MetodoDePagoId { get; set; }
        public string PedidoId { get; set; } = string.Empty;
        public string ClienteId { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }
}
