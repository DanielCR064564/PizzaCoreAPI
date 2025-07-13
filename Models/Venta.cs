using System.ComponentModel.DataAnnotations;

namespace PizzaCoreAPI.Models
{
    public class Venta
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public string NumeroOrden { get; set; } = string.Empty;
        
        public string? ClienteId { get; set; }
        public Usuario? Cliente { get; set; }
        
        public string? EmpleadoId { get; set; }
        public Usuario? Empleado { get; set; }
        
        [Required]
        public DateTime FechaVenta { get; set; } = DateTime.Now;
        
        [Required]
        public decimal Total { get; set; }
        
        [Required]
        public MetodoPago MetodoPago { get; set; } = MetodoPago.Efectivo;
        
        public string? TerminalId { get; set; }
        
        public EstadoVenta Estado { get; set; } = EstadoVenta.Completada;
        
        // Reference to the created Pedido
        public string? PedidoId { get; set; }
        public Pedido? Pedido { get; set; }
        
        // Reference to the created Factura
        public Guid? FacturaId { get; set; }
        public Factura? Factura { get; set; }
        
        public virtual ICollection<VentaDetalle> Detalles { get; set; } = new List<VentaDetalle>();
        
        // Audit fields
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaActualizacion { get; set; }
    }
    
    public class VentaDetalle
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public Guid VentaId { get; set; }
        public Venta Venta { get; set; } = null!;
        
        [Required]
        public Guid ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;
        
        [Required]
        public int Cantidad { get; set; }
        
        [Required]
        public decimal PrecioUnitario { get; set; }
        
        [Required]
        public decimal Subtotal { get; set; }
    }
    
    public enum EstadoVenta
    {
        Pendiente,
        Completada,
        Cancelada,
        Reembolsada
    }
}
