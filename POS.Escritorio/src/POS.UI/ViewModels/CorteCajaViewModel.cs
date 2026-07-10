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

        private decimal _totalEfectivo;
        public decimal TotalEfectivo
        {
            get => _totalEfectivo;
            private set => SetProperty(ref _totalEfectivo, value);
        }

        private decimal _totalTarjeta;
        public decimal TotalTarjeta
        {
            get => _totalTarjeta;
            private set => SetProperty(ref _totalTarjeta, value);
        }

        public decimal EfectivoEsperado => (TurnoActual?.EfectivoInicial ?? 0) + TotalEfectivo;

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

            TotalEfectivo = await _ventaService.ObtenerTotalPorMetodoPagoAsync(
                _sucursalId, TurnoActual.FechaApertura, DateTime.Now, "Efectivo");

            TotalTarjeta = await _ventaService.ObtenerTotalPorMetodoPagoAsync(
                _sucursalId, TurnoActual.FechaApertura, DateTime.Now, "Tarjeta de crédito")
                + await _ventaService.ObtenerTotalPorMetodoPagoAsync(
                _sucursalId, TurnoActual.FechaApertura, DateTime.Now, "Tarjeta de débito");

            TotalVentasTurno = TotalEfectivo + TotalTarjeta; // Solo informativo, ya no se usa para la diferencia
        }

        private async Task CerrarTurnoAsync()
        {
            if (TurnoActual == null) return;
            await ActualizarTotalVentasAsync();
            TurnoActual = await _corteCajaService.CerrarTurnoAsync(TurnoActual.Id, EfectivoFinal);
        }
    }
}
