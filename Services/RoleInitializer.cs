using Microsoft.AspNetCore.Identity;
using PizzaCoreAPI.Models;

namespace PizzaCoreAPI.Services
{
    public class RoleInitializer
    {
        private readonly RoleManager<Rol> _roleManager;

        public RoleInitializer(RoleManager<Rol> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task InitializeAsync()
        {
            string[] roles = new string[] { "Administrador", "Empleado", "Cliente" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new Rol(role));
                }
            }
        }
    }
}
