using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace POS.UI.Converters
{
    public class BooleanoAVisibilidadConverter : IValueConverter
    {
        // Con ConverterParameter="Invertir" muestra el elemento cuando el valor es false.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool visible = value is bool b && b;
            if (parameter as string == "Invertir") visible = !visible;
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}