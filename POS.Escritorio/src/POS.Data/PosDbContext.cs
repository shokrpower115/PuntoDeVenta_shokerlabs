using Microsoft.EntityFrameworkCore;
using POS.Core.Models;
using POS.Core.Seguridad;

namespace POS.Data
{
    public class PosDbContext : DbContext
    {
        public DbSet<Sucursal> Sucursales => Set<Sucursal>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<ProductoSucursal> ProductoSucursales => Set<ProductoSucursal>();
        public DbSet<Venta> Ventas => Set<Venta>();
        public DbSet<VentaDetalle> VentaDetalles => Set<VentaDetalle>();
        public DbSet<CorteDeCaja> CortesCaja => Set<CorteDeCaja>();
        public DbSet<MetodoPago> MetodosPago => Set<MetodoPago>();

        private readonly string _rutaBaseDatos;

        public PosDbContext(string rutaBaseDatos)
        {
            _rutaBaseDatos = rutaBaseDatos;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={_rutaBaseDatos}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Llave compuesta: un producto solo puede tener UN registro de stock
            // por sucursal (no puede repetirse ProductoId+SucursalId).
            modelBuilder.Entity<ProductoSucursal>()
                .HasKey(ps => new { ps.ProductoId, ps.SucursalId });

            // Datos iniciales del catálogo de métodos de pago.
            // Se insertan solos la primera vez que se crea la base de datos.
            modelBuilder.Entity<MetodoPago>().HasData(
                new MetodoPago { Id = 1, Nombre = "Efectivo", Activo = true },
                new MetodoPago { Id = 2, Nombre = "Tarjeta de crédito", Activo = true },
                new MetodoPago { Id = 3, Nombre = "Tarjeta de débito", Activo = true },
                new MetodoPago { Id = 4, Nombre = "Transferencia", Activo = true }
            );

            modelBuilder.Entity<Sucursal>().HasData(
                new Sucursal { Id = 1, Nombre = "Sucursal Centro" },
                new Sucursal { Id = 2, Nombre = "Sucursal Norte" },
                new Sucursal { Id = 3, Nombre = "Sucursal Sur" }
            );

            modelBuilder.Entity<Usuario>().HasData(
                 new Usuario
                 {
                     Id = 1,
                     NombreUsuario = "centro",
                     PasswordHash = HashPassword.Generar("1234"),
                     NombreCompleto = "Moisés",
                     Puesto = "Cajero",
                     Rol = RolUsuario.Cajero,
                     SucursalId = 1
                 },
                 new Usuario
                 {
                     Id = 2,
                     NombreUsuario = "norte",
                     PasswordHash = HashPassword.Generar("1234"),
                     NombreCompleto = "Moisés",
                     Puesto = "Cajero",
                     Rol = RolUsuario.Cajero,
                     SucursalId = 2
                 },
                 new Usuario
                 {
                     Id = 3,
                     NombreUsuario = "sur",
                     PasswordHash = HashPassword.Generar("1234"),
                     NombreCompleto = "Moisés",
                     Puesto = "Cajero",
                     Rol = RolUsuario.Cajero,
                     SucursalId = 3
                 }
            );
        }
    }
}