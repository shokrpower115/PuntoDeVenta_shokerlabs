using POS.Core.Models;
using POS.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace POS.UI.ViewModels
{
    public class CorteCajaViewModel : ViewModelBase
    {
        private readonly ICorteCajaService _corteCajaService;
        private readonly IVentaService _ventaService;
        private readonly int _sucursalId;
        private readonly int _usuarioId;

        private string _mensaje = string.Empty;
        public string Mensaje
        {
            get => _mensaje;
            set => SetProperty(ref _mensaje, value);
        }

        public ObservableCollection<CategoriaMovimientoCaja> CategoriasEntrada { get; } = new();
        public ObservableCollection<CategoriaMovimientoCaja> CategoriasRetiro { get; } = new();


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

        public decimal EfectivoEsperado => TurnoActual?.EfectivoEsperado ?? 0;

        public RelayCommand AbrirTurnoCommand { get; }
        public RelayCommand CerrarTurnoCommand { get; }

        public CorteCajaViewModel(ICorteCajaService corteCajaService, IVentaService ventaService, int sucursalId, int usuarioId)
        {
            _corteCajaService = corteCajaService;
            _ventaService = ventaService;
            _sucursalId = sucursalId;
            _usuarioId = usuarioId;

            AbrirTurnoCommand = new RelayCommand(async () => await AbrirTurnoAsync());
            CerrarTurnoCommand = new RelayCommand(async () => await CerrarTurnoAsync());
            RegistrarEntradaCommand = new RelayCommand(async () => await RegistrarEntradaAsync());
            RegistrarRetiroCommand = new RelayCommand(async () => await RegistrarRetiroAsync());

            _ = CargarTurnoAsync();
            _ = CargarCategoriasAsync();
        }

        private CategoriaMovimientoCaja? _categoriaEntradaSeleccionada;
        public CategoriaMovimientoCaja? CategoriaEntradaSeleccionada
        {
            get => _categoriaEntradaSeleccionada;
            set => SetProperty(ref _categoriaEntradaSeleccionada, value);
        }

        private CategoriaMovimientoCaja? _categoriaRetiroSeleccionada;
        public CategoriaMovimientoCaja? CategoriaRetiroSeleccionada
        {
            get => _categoriaRetiroSeleccionada;
            set => SetProperty(ref _categoriaRetiroSeleccionada, value);
        }

        private string _descripcionOtroEntrada = string.Empty;
        public string DescripcionOtroEntrada
        {
            get => _descripcionOtroEntrada;
            set => SetProperty(ref _descripcionOtroEntrada, value);
        }

        private decimal _montoEntrada;
        public decimal MontoEntrada
        {
            get => _montoEntrada;
            set => SetProperty(ref _montoEntrada, value);
        }

        private string _descripcionOtroRetiro = string.Empty;
        public string DescripcionOtroRetiro
        {
            get => _descripcionOtroRetiro;
            set => SetProperty(ref _descripcionOtroRetiro, value);
        }

        private decimal _montoRetiro;
        public decimal MontoRetiro
        {
            get => _montoRetiro;
            set => SetProperty(ref _montoRetiro, value);
        }

        public RelayCommand RegistrarEntradaCommand { get; }
        public RelayCommand RegistrarRetiroCommand { get; }

        private async Task CargarTurnoAsync()
        {
            TurnoActual = await _corteCajaService.ObtenerTurnoAbiertoAsync(_sucursalId);
            if (TurnoActual != null)
                await ActualizarTotalVentasAsync();
        }

        private async Task AbrirTurnoAsync()
        {
            TurnoActual = await _corteCajaService.AbrirTurnoAsync(_sucursalId, _usuarioId, EfectivoInicial);
            await ActualizarTotalVentasAsync();

            Mensaje = "Ingreso de efectivo con exito.";
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
            TurnoActual = await _corteCajaService.CerrarTurnoAsync(TurnoActual.Id, EfectivoFinal, TotalEfectivo, TotalTarjeta);
        }

        private async Task CargarCategoriasAsync()
        {
            var entradas = await _corteCajaService.ObtenerCategoriasActivasAsync(TipoMovimientoCaja.Entrada);
            CategoriasEntrada.Clear();
            foreach (var c in entradas) CategoriasEntrada.Add(c);

            var retiros = await _corteCajaService.ObtenerCategoriasActivasAsync(TipoMovimientoCaja.Retiro);
            CategoriasRetiro.Clear();
            foreach (var c in retiros) CategoriasRetiro.Add(c);
        }

        private async Task RegistrarEntradaAsync()
        {
            if (TurnoActual == null)
            {
                Mensaje = "Primero debes abrir un turno.";
                return;
            }
            if (CategoriaEntradaSeleccionada == null)
            {
                Mensaje = "Selecciona una categoría.";
                return;
            }
            if (MontoEntrada <= 0)
            {
                Mensaje = "Escribe un monto mayor a 0.";
                return;
            }

            var movimiento = new MovimientoCaja
            {
                CorteCajaId = TurnoActual.Id,
                CategoriaId = CategoriaEntradaSeleccionada.Id,
                DescripcionOtro = CategoriaEntradaSeleccionada.Nombre == "Otro" ? DescripcionOtroEntrada : null,
                Monto = MontoEntrada,
                UsuarioId = _usuarioId
            };

            await _corteCajaService.RegistrarMovimientoAsync(movimiento);

            MontoEntrada = 0;
            DescripcionOtroEntrada = string.Empty;
            await CargarTurnoAsync(); // Recarga el turno para reflejar el movimiento nuevo en los totales
        }

        private async Task RegistrarRetiroAsync()
        {
            if (TurnoActual == null || CategoriaRetiroSeleccionada == null || MontoRetiro <= 0) return;

            var movimiento = new MovimientoCaja
            {
                CorteCajaId = TurnoActual.Id,
                CategoriaId = CategoriaRetiroSeleccionada.Id,
                DescripcionOtro = CategoriaRetiroSeleccionada.Nombre == "Otro" ? DescripcionOtroRetiro : null,
                Monto = MontoRetiro,
                UsuarioId = _usuarioId
            };

            await _corteCajaService.RegistrarMovimientoAsync(movimiento);

            MontoRetiro = 0;
            DescripcionOtroRetiro = string.Empty;
            await CargarTurnoAsync();
        }
    }
}
