using System;
using System.Threading.Tasks;
using POS.Core.Models;
using POS.Core.Services;

namespace POS.UI.ViewModels
{
    public class CorteCajaViewModel : ViewModelBase
    {
        private readonly ICorteCajaService _corteCajaService;
        private readonly IVentaService _ventaService;
        private readonly int _sucursalId;

        private CorteDeCaja? _turnoActual;
        public CorteDeCaja? TurnoActual
        {
            get => _turnoActual;
            set => SetProperty(ref _turnoActual, value);
        }

        private decimal _efectivoInicial;
        public decimal EfectivoInicial
        {
            get => _efectivoInicial;
            set => SetProperty(ref _efectivoInicial, value);
        }

        private decimal _efectivoFinal;
        public decimal EfectivoFinal
        {
            get => _efectivoFinal;
            set => SetProperty(ref _efectivoFinal, value);
        }

        private decimal _totalVentasTurno;
        public decimal TotalVentasTurno
        {
            get => _totalVentasTurno;
            set => SetProperty(ref _totalVentasTurno, value);
        }

        public RelayCommand AbrirTurnoCommand { get; }
        public RelayCommand CerrarTurnoCommand { get; }

        public CorteCajaViewModel(ICorteCajaService corteCajaService, IVentaService ventaService, int sucursalId)
        {
            _corteCajaService = corteCajaService;
            _ventaService = ventaService;
            _sucursalId = sucursalId;

            AbrirTurnoCommand = new RelayCommand(async () => await AbrirTurnoAsync());
            CerrarTurnoCommand = new RelayCommand(async () => await CerrarTurnoAsync());

            _ = CargarTurnoAsync();
        }

        private async Task CargarTurnoAsync()
        {
            TurnoActual = await _corteCajaService.ObtenerTurnoAbiertoAsync(_sucursalId);
            if (TurnoActual != null)
                await ActualizarTotalVentasAsync();
        }

        private async Task AbrirTurnoAsync()
        {
            TurnoActual = await _corteCajaService.AbrirTurnoAsync(_sucursalId, EfectivoInicial);
            await ActualizarTotalVentasAsync();
        }

        private async Task ActualizarTotalVentasAsync()
        {
            if (TurnoActual == null) return;
            TotalVentasTurno = await _ventaService.ObtenerTotalVentasAsync(
                _sucursalId, TurnoActual.FechaApertura, DateTime.Now);
        }

        private async Task CerrarTurnoAsync()
        {
            if (TurnoActual == null) return;
            await ActualizarTotalVentasAsync();
            TurnoActual = await _corteCajaService.CerrarTurnoAsync(TurnoActual.Id, EfectivoFinal);
        }
    }
}
