using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using POS.Core.Models;
using POS.Core.Services;

namespace POS.Data.Sqlite
{
    public class CorteCajaServiceSqlite : ICorteCajaService
    {
        public async Task<CorteDeCaja> AbrirTurnoAsync(int sucursalId, decimal efectivoInicial)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            var corte = new CorteDeCaja
            {
                SucursalId = sucursalId,
                FechaApertura = DateTime.Now,
                EfectivoInicial = efectivoInicial,
                Cerrado = false
            };

            db.CortesCaja.Add(corte);
            await db.SaveChangesAsync();
            return corte;
        }

        public async Task<CorteDeCaja?> ObtenerTurnoAbiertoAsync(int sucursalId)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            return await db.CortesCaja
                .FirstOrDefaultAsync(c => c.SucursalId == sucursalId && !c.Cerrado);
        }

        public async Task<CorteDeCaja> CerrarTurnoAsync(int corteId, decimal efectivoFinal)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            var corte = await db.CortesCaja.FirstAsync(c => c.Id == corteId);
            corte.EfectivoFinal = efectivoFinal;
            corte.FechaCierre = DateTime.Now;
            corte.Cerrado = true;

            await db.SaveChangesAsync();
            return corte;
        }
    }
}