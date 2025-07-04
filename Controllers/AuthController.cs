using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PizzaCoreAPI.Data;
using PizzaCoreAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PizzaCoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly RoleManager<Rol> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            RoleManager<Rol> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("RegisterCliente")]
        public async Task<IActionResult> RegisterCliente([FromBody] RegisterWrapper wrapper)
        {
            try
            {
                var usuario = wrapper.Usuario;
                var password = wrapper.Password;

                if (string.IsNullOrWhiteSpace(password))
                    return BadRequest(new { message = "La contraseña es requerida" });

                if (await _userManager.FindByNameAsync(usuario.UserName) != null)
                    return BadRequest(new { message = "El nombre de usuario ya existe" });

                usuario.EsEmpleado = false;
                usuario.RNC = null;
                usuario.Cargo = null;

                var result = await _userManager.CreateAsync(usuario, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(usuario, "Cliente");
                    return Ok(new { message = "Cliente registrado exitosamente" });
                }

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al registrar cliente", error = ex.Message });
            }
        }

        [HttpPost("RegisterEmpleado")]
        public async Task<IActionResult> RegisterEmpleado([FromBody] Usuario usuario)
        {
            try
            {
                if (string.IsNullOrEmpty(usuario.RNC))
                    return BadRequest(new { message = "El campo RNC es requerido para empleados" });

                if (string.IsNullOrEmpty(usuario.Cargo))
                    return BadRequest(new { message = "El campo Cargo es requerido para empleados" });

                if (await _userManager.FindByNameAsync(usuario.UserName) != null)
                    return BadRequest(new { message = "El nombre de usuario ya existe" });

                var password = usuario.PasswordHash;

                var result = await _userManager.CreateAsync(usuario, password);

                if (result.Succeeded)
                {
                    usuario.EsEmpleado = true;
                    await _userManager.AddToRoleAsync(usuario, "Empleado");
                    return Ok(new { message = "Empleado registrado exitosamente" });
                }

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al registrar empleado", error = ex.Message });
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return BadRequest(new { message = "Credenciales inválidas" });

                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, false);

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    return Ok(new
                    {
                        success = true,
                        email = user.Email,
                        nombre = user.NombreCompleto,
                        roles = roles,
                        token = await GenerateJwtToken(user)
                    });
                }

                return BadRequest(new { message = "Credenciales inválidas" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al iniciar sesión", error = ex.Message });
            }
        }

        [HttpPost("CrearRol")]
        public async Task<IActionResult> CrearRol([FromBody] string nombreRol)
        {
            try
            {
                if (await _roleManager.RoleExistsAsync(nombreRol))
                {
                    return BadRequest(new { message = $"El rol '{nombreRol}' ya existe." });
                }

                var role = new Rol(nombreRol);
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return Ok(new { message = "Rol creado exitosamente" });
                }

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear rol", error = ex.Message });
            }
        }

        [HttpPost("AsignarRol")]
        public async Task<IActionResult> AsignarRol([FromBody] AsignarRolModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return BadRequest(new { message = $"No se encontró el usuario con email '{model.Email}'" });

                var result = await _userManager.AddToRoleAsync(user, model.Rol);

                if (result.Succeeded)
                {
                    return Ok(new { message = "Rol asignado exitosamente" });
                }

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al asignar rol", error = ex.Message });
            }
        }

        private async Task<string> GenerateJwtToken(Usuario user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("nombreCompleto", user.NombreCompleto ?? "")
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class LoginModel
        {
            [Required]
            public string Email { get; set; } = string.Empty;
            [Required]
            public string Password { get; set; } = string.Empty;
        }

        public class RegisterWrapper
        {
            [Required]
            public Usuario Usuario { get; set; } = null!;
            [Required]
            public string Password { get; set; } = string.Empty;
        }

        public class AsignarRolModel
        {
            [Required]
            public string Email { get; set; } = string.Empty;
            [Required]
            public string Rol { get; set; } = string.Empty;
        }
    }
}
