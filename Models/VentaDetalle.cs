using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PizzaCoreAPI.Models
{
    [Table("venta_detalles")]
    public class VentaDetalle
    {
        [Key]
        [Column("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Column("venta_id")]
        [StringLength(36)]
        public string VentaId { get; set; } = string.Empty;

        [Required]
        [Column("producto_id")]
        [StringLength(36)]
        public string ProductoId { get; set; } = string.Empty;

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("precio_unitario", TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column("subtotal", TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navigation properties
        [JsonIgnore]
        public virtual Venta? Venta { get; set; }

        [JsonIgnore]
        public virtual Producto? Producto { get; set; }
    }
}
