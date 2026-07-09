using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using POS.Core.Models;
using POS.Core.Services;

namespace POS.Data.Mock
{
    public class CorteCajaServiceMock : ICorteCajaService
    {
        private static readonly List<CorteDeCaja> _cortes = new();
        private static int _siguienteId = 1;

        public Task<CorteDeCaja> AbrirTurnoAsync(int sucursalId, decimal efectivoInicial)
        {
            var corte = new CorteDeCaja
            {
                Id = _siguienteId++,
                SucursalId = sucursalId,
                FechaApertura = DateTime.Now,
                EfectivoInicial = efectivoInicial,
                Cerrado = false
            };
            _cortes.Add(corte);
            return Task.FromResult(corte);
        }

        public Task<CorteDeCaja?> ObtenerTurnoAbiertoAsync(int sucursalId)
        {
            var corte = _cortes.FirstOrDefault(c => c.SucursalId == sucursalId && !c.Cerrado);
            return Task.FromResult(corte);
        }

        public Task<CorteDeCaja> CerrarTurnoAsync(int corteId, decimal efectivoFinal)
        {
            var corte = _cortes.First(c => c.Id == corteId);
            corte.EfectivoFinal = efectivoFinal;
            corte.FechaCierre = DateTime.Now;
            corte.Cerrado = true;
            return Task.FromResult(corte);
        }
    }
}
