using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Design;

namespace POS.Data
{
    // Esta clase SOLO la usan las herramientas de migración de EF Core
    // (Add-Migration, Update-Database) para saber cómo crear el DbContext
    // sin necesitar arrancar toda la aplicación WPF.
    public class PosDbContextFactory : IDesignTimeDbContextFactory<PosDbContext>
    {
        public PosDbContext CreateDbContext(string[] args)
        {
            return new PosDbContext(RutaBaseDatos.Obtener());
        }
    }
}