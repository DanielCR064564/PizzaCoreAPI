namespace PizzaCoreAPI.DTOs
{
    public class VentaDTO
    {
        public Guid Id { get; set; }
        public string NumeroOrden { get; set; } = string.Empty;
        public string? ClienteId { get; set; }
        public string? ClienteNombre { get; set; }
        public string? EmpleadoId { get; set; }
        public string? EmpleadoNombre { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string? TerminalId { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? PedidoId { get; set; }
        public Guid? FacturaId { get; set; }
        public List<VentaDetalleDTO> Detalles { get; set; } = new();
        public DateTime FechaCreacion { get; set; }
    }

    public class VentaDetalleDTO
    {
        public Guid Id { get; set; }
        public Guid ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class CrearVentaDTO
    {
        public string NumeroOrden { get; set; } = string.Empty;
        public string? ClienteId { get; set; }
        public string? EmpleadoId { get; set; }
        public DateTime FechaVenta { get; set; } = DateTime.Now;
        public decimal Total { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string? TerminalId { get; set; }
        public List<CrearVentaDetalleDTO> Detalles { get; set; } = new();
    }

    public class CrearVentaDetalleDTO
    {
        public Guid ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
