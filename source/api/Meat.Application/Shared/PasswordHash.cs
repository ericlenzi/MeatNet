using System;
using System.Security.Cryptography;
using System.Text;

namespace Meat.Application.Shared
{
    /// <summary>
    /// Hash de contrasenas del sistema.
    ///
    /// Es SHA1 sin salt, que hoy no se considera adecuado para contrasenas. Se centraliza
    /// aca justamente para que migrarlo (a PBKDF2 o BCrypt, rehasheando en el proximo login
    /// de cada usuario) sea un cambio en un solo lugar y no en cada handler.
    /// </summary>
    public static class PasswordHash
    {
        public static string Calcular(string contrasena)
        {
            using var sha1 = SHA1.Create();
            var bytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(contrasena ?? string.Empty));
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }

        public static bool Coincide(string contrasena, string hash) =>
            string.Equals(Calcular(contrasena), hash, StringComparison.OrdinalIgnoreCase);
    }
}
