using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PizzaCoreAPI.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required]
        public string NombreCompleto { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string Telefono { get; set; } = string.Empty;

        public string Direccion { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public string CodigoPostal { get; set; } = string.Empty;
        public bool EsFrecuente { get; set; }
        public string Notas { get; set; } = string.Empty;
        public string RNC { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
