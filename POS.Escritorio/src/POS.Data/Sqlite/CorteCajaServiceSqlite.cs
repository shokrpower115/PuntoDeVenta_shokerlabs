using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using POS.Core.Models;
using POS.Core.Services;

namespace POS.Data.Sqlite
{
    public class CorteCajaServiceSqlite : ICorteCajaService
    {

        public async Task<CorteDeCaja> AbrirTurnoAsync(int sucursalId, int usuarioId, decimal efectivoInicial)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            var corte = new CorteDeCaja
            {
                SucursalId = sucursalId,
                UsuarioId = usuarioId,
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
                .Include(c => c.Movimientos)
                    .ThenInclude(m => m.Categoria)
                .FirstOrDefaultAsync(c => c.SucursalId == sucursalId && !c.Cerrado);
        }

        public async Task<CorteDeCaja> CerrarTurnoAsync(int corteId, decimal efectivoFinal, decimal totalVentasEfectivo, decimal totalVentasTarjeta)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            var corte = await db.CortesCaja.FirstAsync(c => c.Id == corteId);
            corte.EfectivoFinal = efectivoFinal;
            corte.TotalVentasEfectivo = totalVentasEfectivo;
            corte.TotalVentasTarjeta = totalVentasTarjeta;
            corte.FechaCierre = DateTime.Now;
            corte.Cerrado = true;

            await db.SaveChangesAsync();
            return corte;
        }

        public async Task<List<CategoriaMovimientoCaja>> ObtenerCategoriasActivasAsync(TipoMovimientoCaja tipo)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            return await db.CategoriasMovimientoCaja
                .Where(c => c.Tipo == tipo && c.Activo)
                .ToListAsync();
        }

        public async Task<MovimientoCaja> RegistrarMovimientoAsync(MovimientoCaja movimiento)
        {
            using var db = new PosDbContext(RutaBaseDatos.Obtener());

            db.MovimientosCaja.Add(movimiento);
            await db.SaveChangesAsync();
            return movimiento;
        }


    }
}