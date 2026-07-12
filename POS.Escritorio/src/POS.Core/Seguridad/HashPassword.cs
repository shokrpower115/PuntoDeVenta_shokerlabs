using System;
using System.Security.Cryptography;

namespace POS.Core.Seguridad
{
    // Convierte una contraseña en texto plano a un hash seguro para guardar
    // en base de datos, y permite verificar un intento de login contra ese hash.
    // Nunca se guarda ni se compara la contraseña en texto plano.
    public static class HashPassword
    {
        private const int TamanoSal = 16;
        private const int TamanoHash = 32;
        private const int Iteraciones = 100_000;

        public static string Generar(string passwordTextoPlano)
        {
            byte[] sal = RandomNumberGenerator.GetBytes(TamanoSal);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                passwordTextoPlano, sal, Iteraciones, HashAlgorithmName.SHA256, TamanoHash);

            // Guardamos sal + hash juntos, separados por un punto, como un solo string.
            return $"{Convert.ToBase64String(sal)}.{Convert.ToBase64String(hash)}";
        }

        public static bool Verificar(string passwordTextoPlano, string hashGuardado)
        {
            var partes = hashGuardado.Split('.');
            if (partes.Length != 2) return false;

            byte[] sal = Convert.FromBase64String(partes[0]);
            byte[] hashEsperado = Convert.FromBase64String(partes[1]);

            byte[] hashIntento = Rfc2898DeriveBytes.Pbkdf2(
                passwordTextoPlano, sal, Iteraciones, HashAlgorithmName.SHA256, TamanoHash);

            return CryptographicOperations.FixedTimeEquals(hashIntento, hashEsperado);
        }
    }
}