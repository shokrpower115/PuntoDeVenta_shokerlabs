using System.Windows.Controls;
using System.Windows.Input;

namespace POS.UI.Views
{
    public partial class VentaView : UserControl
    {
        public VentaView()
        {
            InitializeComponent();

            // El cursor arranca en el buscador: el cajero puede escanear sin tocar el mouse.
            Loaded += (s, e) => TxtBusqueda.Focus();
            PreviewKeyDown += AlPresionarTecla;
        }

        // F2 regresa al buscador desde cualquier parte de la pantalla de venta.
        private void AlPresionarTecla(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                TxtBusqueda.Focus();
                TxtBusqueda.SelectAll();
                e.Handled = true;
            }
        }
    }
}
