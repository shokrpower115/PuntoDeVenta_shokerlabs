namespace POS.Core.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Puesto { get; set; } = string.Empty;

        // A qué sucursal pertenece este usuario. Esto es lo que reemplaza
        // el "SucursalActualId" que hoy está fijo en MainViewModel.
        public int SucursalId { get; set; }
        public Sucursal? Sucursal { get; set; }
        public string NombreSucursal { get; set; } = string.Empty;
    }
}
