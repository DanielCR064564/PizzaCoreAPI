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

        // Parameterless constructor for EF Tools
        public PizzaDbContext() : base()
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
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<VentaDetalle> VentaDetalles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure basic relationships first
            ConfigureBasicRelationships(modelBuilder);

            // Skip Venta configuration for now - let EF Core use conventions
            // ConfigureVentaEntitiesSimple(modelBuilder);

            // Configure GUID properties for MySQL (use CHAR(36) for better performance)
            ConfigureGuidProperties(modelBuilder);

            // Configure string lengths for MySQL optimization
            ConfigureStringLengths(modelBuilder);

            // Configure decimal precision for MySQL
            ConfigureDecimalPrecision(modelBuilder);

            // Configure enum conversions for MySQL
            ConfigureEnumConversions(modelBuilder);
        }

        private void ConfigureIdentityTables(ModelBuilder modelBuilder)
        {
            // Keep Identity tables as varchar(255) - this is the standard
            // Our business entities will use varchar(36) but foreign keys to Identity will use varchar(255)
        }

        private void ConfigureVentaEntities(ModelBuilder modelBuilder)
        {
            // Configure Venta entity with string ID
            modelBuilder.Entity<Venta>(entity =>
            {
                entity.HasKey(v => v.Id);

                entity.Property(v => v.Id)
                    .HasMaxLength(36)
                    .HasColumnType("char(36)")
                    .IsRequired();

                entity.Property(v => v.NumeroOrden)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(v => v.ClienteId)
                    .HasMaxLength(255)
                    .HasColumnType("varchar(255)");

                entity.Property(v => v.EmpleadoId)
                    .HasMaxLength(255)
                    .HasColumnType("varchar(255)");

                entity.Property(v => v.PedidoId)
                    .HasMaxLength(36)
                    .HasColumnType("varchar(36)");

                entity.Property(v => v.FacturaId)
                    .HasMaxLength(36)
                    .HasColumnType("char(36)");

                entity.Property(v => v.TerminalId)
                    .HasMaxLength(50);

                entity.Property(v => v.MetodoPago)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.Property(v => v.Estado)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.Property(v => v.Total)
                    .HasColumnType("decimal(10,2)");

                // Configure relationships - temporarily disable WithMany to test
                entity.HasOne(v => v.Cliente)
                    .WithMany()
                    .HasForeignKey(v => v.ClienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(v => v.Empleado)
                    .WithMany()
                    .HasForeignKey(v => v.EmpleadoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(v => v.Pedido)
                    .WithMany()
                    .HasForeignKey(v => v.PedidoId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(v => v.Factura)
                    .WithMany()
                    .HasForeignKey(v => v.FacturaId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Configure indexes
                entity.HasIndex(v => v.NumeroOrden).HasDatabaseName("IX_Venta_NumeroOrden");
                entity.HasIndex(v => v.FechaVenta).HasDatabaseName("IX_Venta_FechaVenta");
                entity.HasIndex(v => v.Estado).HasDatabaseName("IX_Venta_Estado");
                entity.HasIndex(v => v.ClienteId).HasDatabaseName("IX_Venta_ClienteId");
                entity.HasIndex(v => v.EmpleadoId).HasDatabaseName("IX_Ventas_EmpleadoId");
                entity.HasIndex(v => v.PedidoId).HasDatabaseName("IX_Ventas_PedidoId");
                entity.HasIndex(v => v.FacturaId).HasDatabaseName("IX_Ventas_FacturaId");
            });

            // Configure VentaDetalle entity - simplified approach
            modelBuilder.Entity<VentaDetalle>(entity =>
            {
                entity.HasKey(vd => vd.Id);

                entity.Property(vd => vd.Id)
                    .HasMaxLength(36)
                    .HasColumnType("char(36)")
                    .IsRequired();

                entity.Property(vd => vd.VentaId)
                    .HasMaxLength(36)
                    .HasColumnType("char(36)")
                    .IsRequired();

                // Configure ProductoId as Guid (matches Producto.Id)
                entity.Property(vd => vd.ProductoId)
                    .HasColumnType("char(36)")
                    .IsRequired();

                entity.Property(vd => vd.PrecioUnitario)
                    .HasColumnType("decimal(10,2)");

                entity.Property(vd => vd.Subtotal)
                    .HasColumnType("decimal(10,2)");

                // Configure relationships - temporarily disable WithMany to test
                entity.HasOne(vd => vd.Venta)
                    .WithMany()
                    .HasForeignKey(vd => vd.VentaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(vd => vd.Producto)
                    .WithMany()
                    .HasForeignKey(vd => vd.ProductoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configure indexes
                entity.HasIndex(vd => vd.VentaId).HasDatabaseName("IX_VentaDetalles_VentaId");
                entity.HasIndex(vd => vd.ProductoId).HasDatabaseName("IX_VentaDetalles_ProductoId");
            });
        }

        private void ConfigureVentaEntitiesSimple(ModelBuilder modelBuilder)
        {
            // Configure Venta - only basic configuration
            modelBuilder.Entity<Venta>()
                .HasKey(v => v.Id);

            modelBuilder.Entity<Venta>()
                .Property(v => v.Id)
                .HasColumnType("char(36)");

            // Disable VentaDetalle configuration
            // modelBuilder.Entity<VentaDetalle>()
            //     .HasKey(vd => vd.Id);

            // modelBuilder.Entity<VentaDetalle>()
            //     .Property(vd => vd.Id)
            //     .HasColumnType("char(36)");

            // modelBuilder.Entity<VentaDetalle>()
            //     .Property(vd => vd.VentaId)
            //     .HasColumnType("char(36)");

            // modelBuilder.Entity<VentaDetalle>()
            //     .Property(vd => vd.ProductoId)
            //     .HasColumnType("char(36)");

            // Configure basic relationship
            // modelBuilder.Entity<VentaDetalle>()
            //     .HasOne(vd => vd.Venta)
            //     .WithMany(v => v.Detalles)
            //     .HasForeignKey(vd => vd.VentaId)
            //     .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureBasicRelationships(ModelBuilder modelBuilder)
        {
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

            // Pedido–Usuario relationships - CRITICAL: explicitly configure both relationships
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Cliente)
                .WithMany(u => u.Pedidos)
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
        }

        private void ConfigureVentaEntitiesMinimal(ModelBuilder modelBuilder)
        {
            // Basic configuration without complex relationships
            modelBuilder.Entity<Venta>().ToTable("Ventas");
            modelBuilder.Entity<VentaDetalle>().ToTable("VentaDetalles");

            // Configure primary keys
            modelBuilder.Entity<Venta>().HasKey(v => v.Id);
            modelBuilder.Entity<VentaDetalle>().HasKey(vd => vd.Id);

            // Configure basic properties
            modelBuilder.Entity<Venta>()
                .Property(v => v.Id)
                .HasColumnType("char(36)");

            modelBuilder.Entity<VentaDetalle>()
                .Property(vd => vd.Id)
                .HasColumnType("char(36)");

            modelBuilder.Entity<VentaDetalle>()
                .Property(vd => vd.ProductoId)
                .HasColumnType("char(36)");
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

            // Foreign key GUID references - EXCLUDE VentaDetalle.ProductoId
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

            // New product fields configuration - fix MySQL default value syntax
            modelBuilder.Entity<Producto>()
                .Property(p => p.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            modelBuilder.Entity<Producto>()
                .Property(p => p.FechaActualizacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Producto>()
                .Property(p => p.ApiId)
                .HasMaxLength(36)
                .HasColumnType("varchar(36)");

            modelBuilder.Entity<Producto>()
                .Property(p => p.Imagen)
                .HasMaxLength(255);

            modelBuilder.Entity<Producto>()
                .Property(p => p.IngredientesTexto)
                .HasColumnType("text");
        }

        private void ConfigureDecimalPrecision(ModelBuilder modelBuilder)
        {
            // Configure decimal properties for MySQL
            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Ingrediente>()
                .Property(i => i.PrecioAdicional)
                .HasColumnType("decimal(8,2)");

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
                .Property(f => f.Total)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Factura>()
                .Property(f => f.IVA)
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
            // Configure enum to string conversions
            modelBuilder.Entity<Producto>()
                .Property(p => p.Tipo)
                .HasConversion<string>()
                .HasMaxLength(50);

            modelBuilder.Entity<Ingrediente>()
                .Property(i => i.Tipo)
                .HasConversion<string>()
                .HasMaxLength(50);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Estado)
                .HasConversion<string>()
                .HasMaxLength(50);

            modelBuilder.Entity<Factura>()
                .Property(f => f.Estado)
                .HasConversion<string>()
                .HasMaxLength(50);

            modelBuilder.Entity<CuentasPorCobrar>()
                .Property(c => c.Estado)
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