using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PizzaCoreAPI.Models
{
    [Table("factura")]
    public class Factura
    {
        [Key]
        [Column("idfactura")]
        public int Id { get; set; }

        [Column("fechaFactura")]
        public DateTime FechaFactura { get; set; } = DateTime.Now;

        [Column("fkcliente")]
        public int FkCliente { get; set; }

        [Column("tipo_entrega")]
        [StringLength(20)]
        public string TipoEntrega { get; set; } = "Local";

        [Column("subtotal", TypeName = "decimal(12,2)")]
        public decimal Subtotal { get; set; } = 0.00m;

        [Column("iva", TypeName = "decimal(12,2)")]
        public decimal IVA { get; set; } = 0.00m;

        [Column("total", TypeName = "decimal(12,2)")]
        public decimal Total { get; set; } = 0.00m;

        [Column("estado")]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente";

        [Column("empleadoId")]
        public int? EmpleadoId { get; set; }

        [Column("fechaCreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Column("notas")]
        public string? Notas { get; set; }

        [Column("sync_completado")]
        public bool SyncCompletado { get; set; } = false;

        [Column("sync_intentos")]
        public int SyncIntentos { get; set; } = 0;

        [Column("sync_ultimo_intento")]
        public DateTime? SyncUltimoIntento { get; set; }

        [Column("sync_error")]
        public string? SyncError { get; set; }

        // No navigation properties to avoid shadow property issues

        // Computed properties for backward compatibility
        [NotMapped]
        public DateTime FechaEmision => FechaFactura;

        [NotMapped]
        public string NumeroFactura => $"F-{Id:D6}";

        [NotMapped]
        public string RNC => "";

        [NotMapped]
        public string NombreCliente => "";

        [NotMapped]
        public string DireccionCliente => "";

        [NotMapped]
        public string NombreEmpleado => "";

        [NotMapped]
        public string RNCEmpleado => "";

        [NotMapped]
        public EstadoFactura EstadoEnum => Estado switch
        {
            "Pendiente" => EstadoFactura.Pendiente,
            "Pagado" => EstadoFactura.Pagado,
            "Cancelado" => EstadoFactura.Anulado,
            _ => EstadoFactura.Pendiente
        };
    }
}
