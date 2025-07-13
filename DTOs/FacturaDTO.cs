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
        public string Estado { get; set; } = "Pendiente"; // ✅ String for frontend compatibility
        public string NombreCliente { get; set; } = string.Empty;
        public string DireccionCliente { get; set; } = string.Empty;
        public string NombreEmpleado { get; set; } = string.Empty;
        public string RNCEmpleado { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
    }

    public class CrearFacturaDTO
    {
        public string PedidoId { get; set; } = string.Empty;
        public string RNC { get; set; } = string.Empty;
        public string Estado { get; set; } = "Pendiente"; // ✅ String, accepts: "Pendiente", "Pagado", "Anulado", "Vencido"
    }
}