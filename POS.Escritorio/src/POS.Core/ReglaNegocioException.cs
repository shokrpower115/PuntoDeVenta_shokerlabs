using System;

namespace POS.Core
{
    // Se lanza cuando una operación viola una regla del negocio (stock insuficiente,
    // no hay turno abierto, usuario duplicado...). El mensaje está pensado para
    // mostrarse tal cual al usuario.
    public class ReglaNegocioException : Exception
    {
        public ReglaNegocioException(string mensaje) : base(mensaje) { }
    }
}
