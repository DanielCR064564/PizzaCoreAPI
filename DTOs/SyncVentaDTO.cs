namespace PizzaCoreAPI.DTOs
{
    public class SyncVentaDTO
    {
        public string NumeroOrden { get; set; } = string.Empty;
        public string? ClienteId { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string? TerminalId { get; set; }
        public List<SyncVentaDetalleDTO> Detalles { get; set; } = new();
    }

    public class SyncVentaDetalleDTO
    {
        public string ProductoApiId { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
