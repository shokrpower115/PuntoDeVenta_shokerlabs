using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using POS.Core.Models;

namespace POS.UI.ViewModels
{
    public class CobroViewModel : ViewModelBase
    {
        public decimal TotalVenta { get; }
        public ObservableCollection<MetodoPago> MetodosPago { get; }

        private MetodoPago? _metodoPagoSeleccionado;
        public MetodoPago? MetodoPagoSeleccionado
        {
            get => _metodoPagoSeleccionado;
            set
            {
                if (SetProperty(ref _metodoPagoSeleccionado, value))
                {
                    OnPropertyChanged(nameof(RequiereMontoPagado));
                    ActualizarCambio();
                }
            }
        }

        private decimal _montoPagado;
        public decimal MontoPagado
        {
            get => _montoPagado;
            set
            {
                if (SetProperty(ref _montoPagado, value))
                    ActualizarCambio();
            }
        }

        private decimal _cambio;
        public decimal Cambio
        {
            get => _cambio;
            private set => SetProperty(ref _cambio, value);
        }

        private string _mensajeError = string.Empty;
        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }

        // Solo en Efectivo tiene sentido preguntar "con cuánto paga" y calcular feria.
        // En tarjeta/vale/transferencia el monto siempre es exacto al total.
        public bool RequiereMontoPagado => MetodoPagoSeleccionado?.Nombre == "Efectivo";

        public event Action? ConfirmadoExitoso;
        public event Action? CancelarSolicitado;

        public RelayCommand ConfirmarCommand { get; }
        public RelayCommand CancelarCommand { get; }

        public CobroViewModel(decimal totalVenta, List<MetodoPago> metodosPago)
        {
            TotalVenta = totalVenta;
            MetodosPago = new ObservableCollection<MetodoPago>(metodosPago);
            MetodoPagoSeleccionado = MetodosPago.FirstOrDefault();
            MontoPagado = totalVenta;

            ConfirmarCommand = new RelayCommand(Confirmar);
            CancelarCommand = new RelayCommand(() => CancelarSolicitado?.Invoke());
        }

        private void ActualizarCambio()
        {
            Cambio = RequiereMontoPagado ? MontoPagado - TotalVenta : 0;
        }

        private void Confirmar()
        {
            MensajeError = string.Empty;

            if (MetodoPagoSeleccionado == null)
            {
                MensajeError = "Selecciona un tipo de pago.";
                return;
            }

            if (RequiereMontoPagado && MontoPagado < TotalVenta)
            {
                MensajeError = "El monto pagado no puede ser menor al total.";
                return;
            }

            if (!RequiereMontoPagado)
                MontoPagado = TotalVenta; // Sin cambio en métodos distintos a efectivo

            ConfirmadoExitoso?.Invoke();
        }
    }
}