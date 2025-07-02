using PizzaCoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DinkToPdf;
using DinkToPdf.Contracts;
using PizzaCoreAPI.Models;
using PizzaCoreAPI.Services;
using PizzaCoreAPI.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configuración del servidor
// Configurar puertos para usar los valores de launchSettings.json
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5200);
    serverOptions.ListenAnyIP(5201, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de DinkToPdf
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

// Configuración de autenticación y base de datos
builder.Services.AddIdentity<Usuario, Rol>()
    .AddEntityFrameworkStores<PizzaDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<PizzaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PizzaDb")));

// Configuración de servicios de negocio
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IMenuService, MenuService>();

// Configuración de inicialización de roles
builder.Services.AddScoped<RoleInitializer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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

// Configuración de autenticación
app.UseAuthentication();
app.UseAuthorization();

// Configuración de rutas
app.MapControllers();

app.Run();
