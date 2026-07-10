using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using POS.Core.Models;
using POS.Core.Services;

namespace POS.Data.Sqlite
{
    public class MetodoPagoServiceSqlite : IMetodoPagoService
    {
        public async Task<List<MetodoPago>> ObtenerActivosAsync()
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            return await db.MetodosPago
                .Where(m => m.Activo)
                .ToListAsync();
        }
    }
}