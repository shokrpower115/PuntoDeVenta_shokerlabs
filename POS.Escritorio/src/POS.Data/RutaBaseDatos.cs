using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Data
{
    public static class RutaBaseDatos
    {
        public static string Obtener()
        {
            var carpeta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PuntoDeVenta");
            Directory.CreateDirectory(carpeta);
            return Path.Combine(carpeta, "pos.db");
        }
    }
}
