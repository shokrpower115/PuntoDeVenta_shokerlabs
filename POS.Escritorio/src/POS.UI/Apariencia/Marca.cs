using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using POS.Core.Models;

namespace POS.UI.Apariencia
{
    // Aplica la identidad del negocio (color, nombre, logo) a los recursos dinámicos
    // de Theme.xaml. Toda la interfaz se actualiza sola porque usa DynamicResource.
    public static class Marca
    {
        // Colores sugeridos en la configuración: todos tienen buen contraste con texto blanco.
        public static readonly string[] ColoresSugeridos =
        {
            "#2563EB", "#0891B2", "#059669", "#65A30D",
            "#D97706", "#DC2626", "#DB2777", "#7C3AED", "#475569"
        };

        public static void Aplicar(DatosNegocio negocio)
        {
            var recursos = Application.Current.Resources;

            var primario = ParsearColor(negocio.ColorPrimario) ?? ParsearColor(DatosNegocio.ColorPrimarioPorDefecto)!.Value;
            recursos["BrushPrimario"] = Congelado(primario);
            recursos["BrushPrimarioOscuro"] = Congelado(Mezclar(primario, Colors.Black, 0.18));
            recursos["BrushPrimarioSuave"] = Congelado(Mezclar(primario, Colors.White, 0.87));
            recursos["BrushPrimarioTexto"] = Congelado(Luminancia(primario) > 0.6 ? Color.FromRgb(0x11, 0x18, 0x27) : Colors.White);

            var nombre = string.IsNullOrWhiteSpace(negocio.Nombre) ? "Punto de Venta" : negocio.Nombre.Trim();
            recursos["NombreNegocio"] = nombre;
            recursos["InicialesNegocio"] = Iniciales(nombre);

            var logo = CrearImagen(negocio.Logo);
            recursos["LogoNegocio"] = logo;
            recursos["VisibilidadLogo"] = logo != null ? Visibility.Visible : Visibility.Collapsed;
            recursos["VisibilidadIniciales"] = logo != null ? Visibility.Collapsed : Visibility.Visible;
        }

        public static Color? ParsearColor(string? hex)
        {
            if (string.IsNullOrWhiteSpace(hex)) return null;
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(hex.Trim());
                return Color.FromRgb(color.R, color.G, color.B); // se ignora la transparencia
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public static BitmapImage? CrearImagen(byte[]? bytes)
        {
            if (bytes is not { Length: > 0 }) return null;
            try
            {
                var imagen = new BitmapImage();
                using var flujo = new MemoryStream(bytes);
                imagen.BeginInit();
                imagen.CacheOption = BitmapCacheOption.OnLoad;
                imagen.StreamSource = flujo;
                imagen.EndInit();
                imagen.Freeze();
                return imagen;
            }
            catch (Exception)
            {
                return null; // Archivo dañado o formato no soportado: se muestran las iniciales.
            }
        }

        private static string Iniciales(string nombre)
        {
            var palabras = nombre.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var iniciales = string.Concat(palabras.Take(2).Select(p => char.ToUpperInvariant(p[0])));
            return iniciales.Length > 0 ? iniciales : "PV";
        }

        private static SolidColorBrush Congelado(Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }

        private static Color Mezclar(Color origen, Color destino, double proporcion) => Color.FromRgb(
            (byte)(origen.R + (destino.R - origen.R) * proporcion),
            (byte)(origen.G + (destino.G - origen.G) * proporcion),
            (byte)(origen.B + (destino.B - origen.B) * proporcion));

        private static double Luminancia(Color c) => (0.299 * c.R + 0.587 * c.G + 0.114 * c.B) / 255;
    }
}
