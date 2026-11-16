using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using POS.UI.Apariencia;

namespace POS.UI.Converters
{
    // "#2563EB" → SolidColorBrush (para las muestras de color de la configuración).
    public class HexABrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => Marca.ParsearColor(value as string) is Color color ? new SolidColorBrush(color) : Brushes.Transparent;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
