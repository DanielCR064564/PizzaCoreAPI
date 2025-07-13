using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaCoreAPI.Models
{
    public class MetodoDePago
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }
}
