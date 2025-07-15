using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PizzaCoreAPI.Models
{
    [Table("ventas")]
    public class Venta
    {
        [Key]
        [Column("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Column("numero_orden")]
        [StringLength(50)]
        public string NumeroOrden { get; set; } = string.Empty;

        [Column("cliente_id")]
        [StringLength(36)]
        public string? ClienteId { get; set; }

        [Column("empleado_id")]
        [StringLength(36)]
        public string? EmpleadoId { get; set; }

        [Column("fecha_venta")]
        public DateTime FechaVenta { get; set; } = DateTime.Now;

        [Column("total", TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        [Column("metodo_pago")]
        public MetodoPago MetodoPago { get; set; } = MetodoPago.Efectivo;

        [Column("terminal_id")]
        [StringLength(50)]
        public string? TerminalId { get; set; }

        [Column("estado")]
        public EstadoVenta Estado { get; set; } = EstadoVenta.Pendiente;

        [Column("pedido_id")]
        [StringLength(36)]
        public string? PedidoId { get; set; }

        [Column("factura_id")]
        [StringLength(36)]
        public string? FacturaId { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Column("fecha_actualizacion")]
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        // Navigation properties
        [JsonIgnore]
        public virtual Cliente? Cliente { get; set; }

        [JsonIgnore]
        public virtual Usuario? Empleado { get; set; }

        [JsonIgnore]
        public virtual Pedido? Pedido { get; set; }

        [JsonIgnore]
        public virtual Factura? Factura { get; set; }

        [JsonIgnore]
        public virtual ICollection<VentaDetalle> Detalles { get; set; } = new List<VentaDetalle>();
    }
}
