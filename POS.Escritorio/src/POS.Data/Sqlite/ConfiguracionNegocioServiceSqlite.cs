using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using POS.Core.Models;
using POS.Core.Services;

namespace POS.Data.Sqlite
{
    public class ConfiguracionNegocioServiceSqlite : IConfiguracionNegocioService
    {
        public async Task<DatosNegocio?> ObtenerAsync()
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            return await db.DatosNegocio
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == DatosNegocio.IdUnico);
        }

        public async Task GuardarAsync(DatosNegocio datos)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            var existente = await db.DatosNegocio.FirstOrDefaultAsync(d => d.Id == DatosNegocio.IdUnico);
            if (existente == null)
            {
                var nuevo = datos.Clonar();
                nuevo.Id = DatosNegocio.IdUnico;
                db.DatosNegocio.Add(nuevo);
            }
            else
            {
                existente.CopiarDe(datos);
            }

            await db.SaveChangesAsync();
        }
    }
}
