using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PizzaCoreAPI.Models;

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

            // Producto–Ingrediente
            modelBuilder.Entity<Producto>()
                .HasMany(p => p.Ingredientes)
                .WithMany(i => i.Productos)
                .UsingEntity(j => j.ToTable("ProductoIngredientes"));

            // Menu–Producto
            modelBuilder.Entity<Menu>()
                .HasMany(m => m.Productos)
                .WithMany(p => p.Menus)
                .UsingEntity(j => j.ToTable("MenuProductos"));

            // ✅ Menu–Pizza (relación agregada)
            modelBuilder.Entity<Menu>()
                .HasMany(m => m.Pizzas)
                .WithMany(p => p.Menus)
                .UsingEntity(j => j.ToTable("MenuPizzas"));

            // PedidoDetalle–Producto
            modelBuilder.Entity<PedidoDetalle>()
                .HasOne(d => d.Producto)
                .WithMany(p => p.PedidoDetalles)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Pedido–Cliente
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Cliente)
                .WithMany(c => c.Pedidos)
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Pedido–Empleado
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Empleado)
                .WithMany(u => u.PedidosEmpleado)
                .HasForeignKey(p => p.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Id)
                .HasConversion<string>();

            modelBuilder.Entity<PedidoDetalle>()
                .HasOne(pd => pd.Pedido)
                .WithMany(p => p.Detalles)
                .HasForeignKey(pd => pd.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PedidoDetalle>()
                .HasOne(pd => pd.Producto)
                .WithMany(p => p.PedidoDetalles)
                .HasForeignKey(pd => pd.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Precision decimal para precios y totales
            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Ingrediente>()
            .Property(i => i.Tipo)
            .HasConversion<string>();

        modelBuilder.Entity<Ingrediente>()
                .Property(i => i.PrecioAdicional)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Total)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PedidoDetalle>()
                .Property(pd => pd.PrecioUnitario)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PedidoDetalle>()
                .Property(pd => pd.Subtotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Factura>()
                .Property(f => f.Subtotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Factura>()
                .Property(f => f.IVA)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Factura>()
                .Property(f => f.Total)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Menu>()
                .Property(m => m.PrecioTotal)
                .HasPrecision(18, 2);
        }
    }
}
