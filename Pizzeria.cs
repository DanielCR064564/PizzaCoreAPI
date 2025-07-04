using PizzaCoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DinkToPdf;
using DinkToPdf.Contracts;
using PizzaCoreAPI.Models;
using PizzaCoreAPI.Services;
using PizzaCoreAPI.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuración de puertos
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5200);
    serverOptions.ListenAnyIP(5201, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});

// Leer configuración JWT desde appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings.GetValue<string>("SecretKey");

// Configuración de servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DinkToPdf
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

// Identity y EF Core
builder.Services.AddIdentity<Usuario, Rol>()
    .AddEntityFrameworkStores<PizzaDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<PizzaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PizzaDb")));

// Autenticación JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
    };
});

// Inyección de servicios personalizados
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<RoleInitializer>();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Inicializar roles
using (var scope = app.Services.CreateScope())
{
    var roleInitializer = scope.ServiceProvider.GetRequiredService<RoleInitializer>();
    await roleInitializer.InitializeAsync();
}

// Middleware de autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Mapear rutas
app.MapControllers();

app.Run();
