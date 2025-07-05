using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PizzaCoreAPI.Models;
using PizzaCoreAPI.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PizzaCoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // POST: api/Auth/RegisterCliente
        [HttpPost("RegisterCliente")]
        public async Task<IActionResult> RegisterCliente([FromBody] RegisterClienteDTO request)
        {
            var existingUser = await _userManager.FindByNameAsync(request.UserName);
            if (existingUser != null)
                return BadRequest(new { message = "El nombre de usuario ya existe" });

            var usuario = new Usuario
            {
                UserName = request.UserName,
                Email = request.Email,
                NombreCompleto = request.NombreCompleto,
                Direccion = request.Direccion,
                PhoneNumber = request.Telefono,
                EsEmpleado = false
            };

            var result = await _userManager.CreateAsync(usuario, request.Password);
            if (!result.Succeeded)
                return BadRequest(new { message = "Error al crear el usuario", errors = result.Errors });

            await _userManager.AddToRoleAsync(usuario, "Cliente");

            var token = GenerateJwtToken(usuario, "Cliente");

            return Ok(new
            {
                message = "Cliente registrado exitosamente",
                token
            });
        }

        // POST: api/Auth/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
                return Unauthorized(new { message = "Credenciales inválidas" });

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new { message = "Credenciales inválidas" });

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles.FirstOrDefault() ?? "Cliente");

            return Ok(new
            {
                message = "Login exitoso",
                token
            });
        }

        // Método privado para generar el token JWT
        private string GenerateJwtToken(Usuario user, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings.GetValue<string>("SecretKey")!;
            var issuer = jwtSettings.GetValue<string>("Issuer")!;
            var audience = jwtSettings.GetValue<string>("Audience")!;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
