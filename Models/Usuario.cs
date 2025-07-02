using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PizzaCoreAPI.Models
{
    public class Usuario : IdentityUser
    {
        [Required]
        public string NombreCompleto { get; set; } = string.Empty;
        
        public string RNC { get; set; } = string.Empty; // Solo para empleados
        
        public string Cargo { get; set; } = string.Empty; // Solo para empleados
        
        public string Direccion { get; set; } = string.Empty;
        
        public bool EsEmpleado { get; set; }
        
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
        public virtual ICollection<Pedido> PedidosEmpleado { get; set; } = new List<Pedido>();
    }
}
