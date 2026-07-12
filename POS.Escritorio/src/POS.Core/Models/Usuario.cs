namespace POS.Core.Models
{
    public enum RolUsuario
    {
        Cajero,
        Administrador
    }

    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public string NombreCompleto { get; set; } = string.Empty;
        public string Puesto { get; set; } = string.Empty;
        public RolUsuario Rol { get; set; } = RolUsuario.Cajero;

        public int SucursalId { get; set; }
        public Sucursal? Sucursal { get; set; }
    }
}