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
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

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

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings.GetValue<string>("SecretKey");

// ✅ Controladores + manejo de ciclos JSON
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// ✅ Swagger con autenticación JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PizzaCore API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese 'Bearer' seguido de su token JWT. Ejemplo: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Conversión a PDF
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

// Identity
builder.Services.AddIdentity<Usuario, Rol>()
    .AddEntityFrameworkStores<PizzaDbContext>()
    .AddDefaultTokenProviders();

// DB Context
builder.Services.AddDbContext<PizzaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PizzaDb")));

// JWT
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

// Servicios personalizados
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IPagoService, PagoService>();
builder.Services.AddScoped<IMetodoDePagoService, MetodoDePagoService>();
builder.Services.AddScoped<ICuentasPorCobrarService, CuentasPorCobrarService>();
builder.Services.AddScoped<RoleInitializer>();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Inicializar roles
using (var scope = app.Services.CreateScope())
{
    var roleInitializer = scope.ServiceProvider.GetRequiredService<RoleInitializer>();
    await roleInitializer.InitializeAsync();
}

// Map controllers
app.MapControllers();

app.Run();
