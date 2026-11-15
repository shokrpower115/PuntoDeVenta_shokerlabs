namespace POS.Core.Models
{
    public enum RolUsuario
    {
        Cajero,
        Administrador,
        SuperAdministrador // reservado para cuando haya múltiples sucursales con la WebAPI
    }

    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public string NombreCompleto { get; set; } = string.Empty;
        public string Puesto { get; set; } = string.Empty;
        public RolUsuario Rol { get; set; } = RolUsuario.Cajero;

        // Se pide cambiar la contraseña en el siguiente inicio de sesión
        // (por ejemplo, el administrador inicial que viene con contraseña por defecto).
        public bool DebeCambiarPassword { get; set; }

        public bool EsAdministrador => Rol is RolUsuario.Administrador or RolUsuario.SuperAdministrador;

        public int SucursalId { get; set; }
        public Sucursal? Sucursal { get; set; }
    }
}