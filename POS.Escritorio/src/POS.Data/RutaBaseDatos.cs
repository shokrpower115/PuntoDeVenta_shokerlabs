using System;
using System.IO;

namespace POS.Data
{
    public static class RutaBaseDatos
    {
        // Permite apuntar a otra base de datos (pruebas, instalaciones personalizadas).
        public const string VariableEntorno = "POS_DB_PATH";

        private static readonly Lazy<string> _ruta = new(Resolver);

        public static string Obtener() => _ruta.Value;

        public static string CarpetaDatos()
        {
            // %LocalAppData% y no "Mis Documentos": Documentos suele estar sincronizado
            // con OneDrive, y la sincronización puede corromper un archivo SQLite abierto.
            var carpeta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PuntoDeVenta");
            Directory.CreateDirectory(carpeta);
            return carpeta;
        }

        private static string Resolver()
        {
            var personalizada = Environment.GetEnvironmentVariable(VariableEntorno);
            if (!string.IsNullOrWhiteSpace(personalizada))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(personalizada))!);
                return personalizada;
            }

            var ruta = Path.Combine(CarpetaDatos(), "pos.db");

            // Versiones anteriores guardaban la base en Documentos: se copia una sola vez
            // y el original se queda donde estaba, como respaldo.
            var rutaAnterior = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PuntoDeVenta", "pos.db");
            if (!File.Exists(ruta) && File.Exists(rutaAnterior))
                File.Copy(rutaAnterior, ruta);

            return ruta;
        }
    }
}
