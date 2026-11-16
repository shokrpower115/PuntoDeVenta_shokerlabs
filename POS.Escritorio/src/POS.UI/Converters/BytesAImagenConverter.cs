using System;
using System.Globalization;
using System.Windows.Data;
using POS.UI.Apariencia;

namespace POS.UI.Converters
{
    // Logo guardado como byte[] → imagen para mostrarla en la configuración.
    public class BytesAImagenConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => Marca.CrearImagen(value as byte[]);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
