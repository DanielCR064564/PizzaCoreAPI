using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PizzaCoreAPI.Models;
using Pomelo.EntityFrameworkCore.MySql;

namespace PizzaCoreAPI.Data
{
    public class PizzaDbContext : IdentityDbContext<Usuario, Rol, string>
    {
        public PizzaDbContext(DbContextOptions<PizzaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Ingrediente> Ingredientes { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoDetalle> PedidoDetalles { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<Pizza> Pizzas { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<MetodoDePago> MetodosDePago { get; set; }
        public DbSet<CuentasPorCobrar> CuentasPorCobrar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Identity tables to use varchar(36) for consistency
            ConfigureIdentityTables(modelBuilder);

            // Configure GUID properties for MySQL (use CHAR(36) for better performance)
            ConfigureGuidProperties(modelBuilder);

            // Configure string lengths for MySQL optimization
            ConfigureStringLengths(modelBuilder);

            // Configure decimal precision for MySQL
            ConfigureDecimalPrecision(modelBuilder);

            // Configure enum conversions for MySQL
            ConfigureEnumConversions(modelBuilder);

            // Producto–Ingrediente (Many-to-Many)
            modelBuilder.Entity<Producto>()
                .HasMany(p => p.Ingredientes)
                .WithMany(i => i.Productos)
                .UsingEntity(j => j.ToTable("ProductoIngredientes"));

            // Menu–Producto (Many-to-Many)
            modelBuilder.Entity<Menu>()
                .HasMany(m => m.Productos)
                .WithMany(p => p.Menus)
                .UsingEntity(j => j.ToTable("MenuProductos"));

            // Menu–Pizza (Many-to-Many)
            modelBuilder.Entity<Menu>()
                .HasMany(m => m.Pizzas)
                .WithMany(p => p.Menus)
                .UsingEntity(j => j.ToTable("MenuPizzas"));

            // PedidoDetalle relationships
            modelBuilder.Entity<PedidoDetalle>()
                .HasOne(d => d.Producto)
                .WithMany(p => p.PedidoDetalles)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PedidoDetalle>()
                .HasOne(pd => pd.Pedido)
                .WithMany(p => p.Detalles)
                .HasForeignKey(pd => pd.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Pedido–Usuario relationships
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Cliente)
                .WithMany(c => c.Pedidos)
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Empleado)
                .WithMany(u => u.PedidosEmpleado)
                .HasForeignKey(p => p.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Factura relationship
            modelBuilder.Entity<Factura>()
                .HasOne(f => f.Pedido)
                .WithMany()
                .HasForeignKey(f => f.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add indexes for frequently queried fields
            ConfigureIndexes(modelBuilder);

            // Configure table engines and charset
            ConfigureTableOptions(modelBuilder);
        }

        private void ConfigureIdentityTables(ModelBuilder modelBuilder)
        {
            // Keep Identity tables as varchar(255) - this is the standard
            // Our business entities will use varchar(36) but foreign keys to Identity will use varchar(255)
            
            // No changes needed here - let Identity use its default configuration
        }

        private void ConfigureGuidProperties(ModelBuilder modelBuilder)
        {
            // Configure all GUID properties to use CHAR(36) for MySQL
            modelBuilder.Entity<Producto>()
                .Property(p => p.Id)
                .HasColumnType("char(36)");

            modelBuilder.Entity<Ingrediente>()
                .Property(i => i.Id)
                .HasColumnType("char(36)");

            modelBuilder.Entity<Menu>()
                .Property(m => m.Id)
                .HasColumnType("char(36)");

            modelBuilder.Entity<Pizza>()
                .Property(p => p.Id)
                .HasColumnType("char(36)");

            modelBuilder.Entity<Factura>()
                .Property(f => f.Id)
                .HasColumnType("char(36)");

            modelBuilder.Entity<MetodoDePago>()
                .Property(m => m.Id)
                .HasColumnType("char(36)");

            // Foreign key GUID references
            modelBuilder.Entity<PedidoDetalle>()
                .Property(pd => pd.ProductoId)
                .HasColumnType("char(36)");

            modelBuilder.Entity<Pago>()
                .Property(p => p.MetodoDePagoId)
                .HasColumnType("char(36)");
        }

        private void ConfigureStringLengths(ModelBuilder modelBuilder)
        {
            // Note: Identity tables use varchar(255) for IDs, so foreign keys should match
            
            // Pedido ID (string/GUID)
            modelBuilder.Entity<Pedido>()
                .Property(p => p.Id)
                .HasMaxLength(36)
                .HasColumnType("varchar(36)");

            // Foreign keys that reference Usuario.Id should match Identity's varchar(255)
            modelBuilder.Entity<Pedido>()
                .Property(p => p.ClienteId)
                .HasMaxLength(255)
                .HasColumnType("varchar(255)");

            modelBuilder.Entity<Pedido>()
                .Property(p => p.EmpleadoId)
                .HasMaxLength(255)
                .HasColumnType("varchar(255)");

            modelBuilder.Entity<PedidoDetalle>()
                .Property(pd => pd.Id)
                .HasMaxLength(36)
                .HasColumnType("varchar(36)");

            modelBuilder.Entity<PedidoDetalle>()
                .Property(pd => pd.PedidoId)
                .HasMaxLength(36)
                .HasColumnType("varchar(36)");

            // CuentasPorCobrar and Pago IDs
            modelBuilder.Entity<CuentasPorCobrar>()
                .Property(c => c.Id)
                .HasMaxLength(36)
                .HasColumnType("varchar(36)");

            // Foreign keys in CuentasPorCobrar - ClienteId references Usuario.Id (varchar(255))
            modelBuilder.Entity<CuentasPorCobrar>()
                .Property(c => c.ClienteId)
                .HasMaxLength(255)
                .HasColumnType("varchar(255)");

            modelBuilder.Entity<CuentasPorCobrar>()
                .Property(c => c.PedidoId)
                .HasMaxLength(36)
                .HasColumnType("varchar(36)");

            modelBuilder.Entity<Pago>()
                .Property(p => p.Id)
                .HasMaxLength(36)
                .HasColumnType("varchar(36)");

            // Foreign keys in Pago
            modelBuilder.Entity<Pago>()
                .Property(p => p.PedidoId)
                .HasMaxLength(36)
                .HasColumnType("varchar(36)");

            modelBuilder.Entity<Pago>()
                .Property(p => p.ClienteId)
                .HasMaxLength(255)
                .HasColumnType("varchar(255)");

            // String properties with reasonable lengths
            modelBuilder.Entity<Producto>()
                .Property(p => p.Nombre)
                .HasMaxLength(200);

            modelBuilder.Entity<Producto>()
                .Property(p => p.Descripcion)
                .HasMaxLength(1000);

            modelBuilder.Entity<Menu>()
                .Property(m => m.Nombre)
                .HasMaxLength(200);

            modelBuilder.Entity<Menu>()
                .Property(m => m.Descripcion)
                .HasMaxLength(1000);

            modelBuilder.Entity<Pizza>()
                .Property(p => p.Nombre)
                .HasMaxLength(200);

            modelBuilder.Entity<Pizza>()
                .Property(p => p.Descripcion)
                .HasMaxLength(1000);

            modelBuilder.Entity<Ingrediente>()
                .Property(i => i.Nombre)
                .HasMaxLength(150);

            modelBuilder.Entity<Ingrediente>()
                .Property(i => i.Descripcion)
                .HasMaxLength(500);

            modelBuilder.Entity<MetodoDePago>()
                .Property(m => m.Nombre)
                .HasMaxLength(100);

            modelBuilder.Entity<MetodoDePago>()
                .Property(m => m.Descripcion)
                .HasMaxLength(300);

            // Usuario properties
            modelBuilder.Entity<Usuario>()
                .Property(u => u.NombreCompleto)
                .HasMaxLength(200);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Direccion)
                .HasMaxLength(500);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.RNC)
                .HasMaxLength(20);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Cargo)
                .HasMaxLength(100);
        }

        private void ConfigureDecimalPrecision(ModelBuilder modelBuilder)
        {
            // MySQL decimal precision (10,2) - good for currency
            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Ingrediente>()
                .Property(i => i.PrecioAdicional)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Total)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<PedidoDetalle>()
                .Property(pd => pd.PrecioUnitario)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<PedidoDetalle>()
                .Property(pd => pd.Subtotal)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Factura>()
                .Property(f => f.Subtotal)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Factura>()
                .Property(f => f.IVA)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Factura>()
                .Property(f => f.Total)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Menu>()
                .Property(m => m.PrecioTotal)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Pizza>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Pago>()
                .Property(p => p.Monto)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<CuentasPorCobrar>()
                .Property(c => c.MontoPendiente)
                .HasColumnType("decimal(10,2)");
        }

        private void ConfigureEnumConversions(ModelBuilder modelBuilder)
        {
            // Convert enums to strings for MySQL compatibility
            modelBuilder.Entity<Producto>()
                .Property(p => p.Tipo)
                .HasConversion<string>()
                .HasMaxLength(50);

            modelBuilder.Entity<Ingrediente>()
                .Property(i => i.Tipo)
                .HasConversion<string>()
                .HasMaxLength(50);
        }

        private void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            // Add indexes for better query performance
            modelBuilder.Entity<Pedido>()
                .HasIndex(p => p.FechaPedido)
                .HasDatabaseName("IX_Pedido_FechaPedido");

            modelBuilder.Entity<Pedido>()
                .HasIndex(p => p.Estado)
                .HasDatabaseName("IX_Pedido_Estado");

            modelBuilder.Entity<Pedido>()
                .HasIndex(p => p.ClienteId)
                .HasDatabaseName("IX_Pedido_ClienteId");

            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.Tipo)
                .HasDatabaseName("IX_Producto_Tipo");

            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.Disponible)
                .HasDatabaseName("IX_Producto_Disponible");

            modelBuilder.Entity<Menu>()
                .HasIndex(m => m.Activo)
                .HasDatabaseName("IX_Menu_Activo");

            modelBuilder.Entity<Factura>()
                .HasIndex(f => f.FechaEmision)
                .HasDatabaseName("IX_Factura_FechaEmision");

            modelBuilder.Entity<Factura>()
                .HasIndex(f => f.Estado)
                .HasDatabaseName("IX_Factura_Estado");
        }

        private void ConfigureTableOptions(ModelBuilder modelBuilder)
        {
            // Remove custom charset and collation configurations to let Identity use defaults
            // This avoids collation conflicts between Identity tables and business tables
        }

        // Override for MySQL date/time handling
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // This will be overridden by DI configuration, but good to have as fallback
                optionsBuilder.UseMySql(
                    "Server=localhost;Database=pizzacoredb;Uid=root;Pwd=root;",
                    ServerVersion.AutoDetect("Server=localhost;Database=pizzacoredb;Uid=root;Pwd=root;"),
                    options => options.EnableRetryOnFailure());
            }
        }
    }
}