using System.ComponentModel.DataAnnotations;

namespace PizzaCoreAPI.DTOs
{
    public class FacturaDTO
    {
        public Guid Id { get; set; }
        public string PedidoId { get; set; } = string.Empty;
        public string NumeroFactura { get; set; } = string.Empty;
        public string RNC { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public string NombreCliente { get; set; } = string.Empty;
        public string DireccionCliente { get; set; } = string.Empty;
        public string NombreEmpleado { get; set; } = string.Empty;
        public string RNCEmpleado { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        public List<PedidoDetalleDTO> Detalles { get; set; } = new();

        // Additional calculated fields
        public decimal TotalSinImpuestos => Subtotal;
        public decimal PorcentajeIVA => Subtotal > 0 ? (IVA / Subtotal) * 100 : 0;
    }

    public class CrearFacturaDTO
    {
        [Required(ErrorMessage = "PedidoId es requerido")]
        public string PedidoId { get; set; } = string.Empty;

        [Required(ErrorMessage = "RNC es requerido")]
        [StringLength(50, ErrorMessage = "RNC no puede exceder 50 caracteres")]
        public string RNC { get; set; } = string.Empty;

        public string? Observaciones { get; set; }
    }

    public class ActualizarEstadoFacturaDTO
    {
        [Required(ErrorMessage = "Estado es requerido")]
        [RegularExpression("^(Pendiente|Pagado|Anulado|Vencido)$",
            ErrorMessage = "Estado debe ser: Pendiente, Pagado, Anulado, o Vencido")]
        public string Estado { get; set; } = string.Empty;

        public string? Motivo { get; set; }
    }

    public class ActualizarFacturaDTO
    {
        [StringLength(50, ErrorMessage = "RNC no puede exceder 50 caracteres")]
        public string? RNC { get; set; }

        [RegularExpression("^(Pendiente|Pagado|Anulado|Vencido)$",
            ErrorMessage = "Estado debe ser: Pendiente, Pagado, Anulado, o Vencido")]
        public string? Estado { get; set; }

        public string? Observaciones { get; set; }
    }
}