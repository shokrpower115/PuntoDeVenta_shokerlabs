using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace POS.UI.ViewModels
{
    // Toda pantalla (VentaViewModel, InventarioViewModel, etc.) hereda de aquí.
    // Esto es lo que permite que cuando cambias una propiedad en C#,
    // la ventana (XAML) se actualice sola sin que tengas que tocar la UI a mano.
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetProperty<T>(ref T campo, T valor, [CallerMemberName] string? nombrePropiedad = null)
        {
            if (Equals(campo, valor)) return false;
            campo = valor;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombrePropiedad));
            return true;
        }

        // Dispara manualmente el evento de cambio para propiedades calculadas
        // (sin campo propio) que dependen de otra propiedad — como RequiereMontoPagado,
        // que depende de MetodoPagoSeleccionado.
        protected void OnPropertyChanged(string nombrePropiedad)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombrePropiedad));
        }
    }
}
