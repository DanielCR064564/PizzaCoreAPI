namespace PizzaCoreAPI.DTOs
{
    public class PedidoDTO
    {
        public string Id { get; set; } = string.Empty;
        public DateTime FechaPedido { get; set; }
        public string Estado { get; set; }
        public string ClienteId { get; set; } = string.Empty;
        public string? EmpleadoId { get; set; }
        public decimal Total { get; set; }
        public List<PedidoDetalleDTO> Detalles { get; set; } = new();
    }

    public class PedidoDetalleDTO
    {
        public string Id { get; set; } = string.Empty;
        public Guid ProductoId { get; set; }
        public int Cantidad { get; set; } = 0;  // ⚠️ Se inicializa para evitar CS8618
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public ProductoDTO Producto { get; set; } = new();
    }

    public class CrearPedidoDTO
    {
        public string ClienteId { get; set; } = string.Empty;
        public string? EmpleadoId { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public List<CrearPedidoDetalleDTO> Detalles { get; set; } = new();
    }

    public class CrearPedidoDetalleDTO
    {
        public Guid ProductoId { get; set; }
        public int Cantidad { get; set; } = 1;
    }
}
