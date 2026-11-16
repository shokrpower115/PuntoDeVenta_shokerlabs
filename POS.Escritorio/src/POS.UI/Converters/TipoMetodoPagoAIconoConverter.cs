using System;
using System.Globalization;
using System.Windows.Data;
using POS.Core.Models;

namespace POS.UI.Converters
{
    // Glifo de "Segoe MDL2 Assets" para cada tipo de pago. El "$" de efectivo se dibuja con
    // la fuente de texto (CobroWindow cambia la fuente para ese caso).
    public class TipoMetodoPagoAIconoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value switch
        {
            TipoMetodoPago.Efectivo => "$",
            TipoMetodoPago.Tarjeta => "",
            TipoMetodoPago.Transferencia => "",
            _ => ""
        };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
