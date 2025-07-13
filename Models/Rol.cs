using Microsoft.AspNetCore.Identity;

namespace PizzaCoreAPI.Models
{
    public class Rol : IdentityRole
    {
        public Rol() : base() { }
        public Rol(string name) : base(name) { }
    }
}
