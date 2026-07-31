using System;
using System.Globalization;
using System.Windows.Data;

namespace POS.UI.Converters
{
    public class DecimalSeguroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is decimal d ? d.ToString("N2", culture) : "0.00";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var texto = value?.ToString()?.Trim();
            if (string.IsNullOrEmpty(texto)) return 0m;
            return decimal.TryParse(texto, NumberStyles.Number, culture, out decimal resultado) ? resultado : 0m;
        }
    }
}