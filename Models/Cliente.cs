using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PizzaCoreAPI.Models
{
    /// <summary>
    /// Tabla de clientes del sistema POS - Updated to match database structure
    /// </summary>
    [Table("cliente")]
    public class Cliente
    {
        [Key]
        [Column("IdCliente")]
        public int IdCliente { get; set; }

        [Required]
        [Column("Nombres")]
        [StringLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        [Column("ApPaterno")]
        [StringLength(50)]
        public string ApPaterno { get; set; } = string.Empty;

        [Required]
        [Column("ApMaterno")]
        [StringLength(50)]
        public string ApMaterno { get; set; } = string.Empty;

        [EmailAddress]
        [Column("Email")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Phone]
        [Column("Telefono")]
        [StringLength(20)]
        public string? Telefono { get; set; }

        [Column("Direccion")]
        [StringLength(200)]
        public string? Direccion { get; set; }

        [Column("Ciudad")]
        [StringLength(50)]
        public string? Ciudad { get; set; }

        [Column("Provincia")]
        [StringLength(50)]
        public string? Provincia { get; set; }

        [Column("CodigoPostal")]
        [StringLength(10)]
        public string? CodigoPostal { get; set; }

        /// <summary>
        /// RNC o Cédula del cliente
        /// </summary>
        [Column("Rnc")]
        [StringLength(15)]
        public string? Rnc { get; set; }

        [Column("EsFrecuente")]
        public bool? EsFrecuente { get; set; }

        [Column("Notas")]
        [StringLength(500)]
        public string? Notas { get; set; }

        [Column("FechaRegistro")]
        public DateTime? FechaRegistro { get; set; }

        [Column("TotalCompras", TypeName = "decimal(10,2)")]
        public decimal? TotalCompras { get; set; }

        [Column("NumeroOrdenes")]
        public int? NumeroOrdenes { get; set; }

        [Column("Activo")]
        public bool? Activo { get; set; } = true;

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

        [JsonIgnore]
        public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

        // Computed property for backward compatibility
        [NotMapped]
        public string NombreCompleto => $"{Nombres} {ApPaterno} {ApMaterno}".Trim();

        // Helper property for compatibility with existing code
        [NotMapped]
        public int Id
        {
            get => IdCliente;
            set => IdCliente = value;
        }
    }
}
