using System;
using System.Globalization;
using System.Windows.Data;

namespace POS.UI.Converters
{
    public class EnumABooleanoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value?.ToString() == parameter?.ToString();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool)value ? Enum.Parse(targetType, parameter!.ToString()!) : Binding.DoNothing;
    }
}