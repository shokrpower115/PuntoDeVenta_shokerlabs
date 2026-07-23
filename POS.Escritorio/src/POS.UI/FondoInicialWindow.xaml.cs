using POS.Core.Services;
using POS.UI.ViewModels;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;

namespace POS.UI
{
    public partial class FondoInicialWindow : Window
    {
        private readonly ICorteCajaService _corteCajaService;
        private readonly IVentaService _ventaService;
        private readonly int _sucursalId;
        private readonly int _usuarioId;

        public FondoInicialWindow(ICorteCajaService corteCajaService, IVentaService ventaService, int sucursalId, int usuarioId)
        {
            InitializeComponent();
            _corteCajaService = corteCajaService;
            _ventaService = ventaService;
            _sucursalId = sucursalId;
            _usuarioId = usuarioId;
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private async void AbrirTurno_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtMonto.Text) ||
                !decimal.TryParse(TxtMonto.Text, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.CurrentCulture, out decimal monto) ||
                monto < 0)
            {
                MessageBox.Show("Escribe un monto válido mayor o igual a 0.", "Fondo inicial", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Abrir turno y guardar en BD
                var corte = await _corteCajaService.AbrirTurnoAsync(_sucursalId, _usuarioId, monto);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir turno: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}