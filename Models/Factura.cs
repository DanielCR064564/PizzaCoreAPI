using System.ComponentModel.DataAnnotations;

namespace PizzaCoreAPI.Models
{
    public class Factura
    {
        [Key]
        public int Id { get; set; }
        
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; } = null!;
        
        public string NumeroFactura { get; set; } = string.Empty;
        public string RNC { get; set; } = string.Empty;
        
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Pagado, Anulado
        
        public string NombreCliente { get; set; } = string.Empty;
        public string DireccionCliente { get; set; } = string.Empty;
        
        public string NombreEmpleado { get; set; } = string.Empty;
        public string RNCEmpleado { get; set; } = string.Empty;
        
        public DateTime FechaEmision { get; set; } = DateTime.Now;
        
        public virtual ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();
    }
}
