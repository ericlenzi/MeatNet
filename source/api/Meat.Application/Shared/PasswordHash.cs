using System;
using System.Security.Cryptography;
using System.Text;

namespace Meat.Application.Shared
{
    /// <summary>Resultado de verificar una contrasena contra el hash guardado.</summary>
    public readonly struct VerificacionContrasena
    {
        public VerificacionContrasena(bool esValida, bool necesitaRehash)
        {
            this.EsValida = esValida;
            this.NecesitaRehash = necesitaRehash;
        }

        public bool EsValida { get; }

        /// <summary>
        /// La contrasena es correcta pero el hash guardado esta en el formato viejo.
        /// Quien verifica deberia reemplazarlo por uno nuevo y guardarlo.
        /// </summary>
        public bool NecesitaRehash { get; }
    }

    /// <summary>
    /// Hash de contrasenas: PBKDF2-HMAC-SHA256 con salt por usuario.
    ///
    /// El sistema venia usando SHA1 sin salt, que no sirve para contrasenas: es rapido de
    /// calcular (justo lo contrario de lo que se busca) y sin salt dos usuarios con la misma
    /// clave tienen el mismo hash, asi que una tabla precalculada las revela todas.
    ///
    /// La migracion es perezosa y nadie tiene que cambiar su contrasena: <see cref="Verificar"/>
    /// reconoce los hashes viejos, los acepta y avisa con <see cref="VerificacionContrasena.NecesitaRehash"/>.
    /// El login rehashea en ese momento, cuando tiene la contrasena en claro. Un hash viejo
    /// desaparece la proxima vez que su dueno entra.
    /// </summary>
    public static class PasswordHash
    {
        private const string Etiqueta = "PBKDF2";
        private const char Separador = '$';

        /// <summary>Recomendacion de OWASP para PBKDF2-HMAC-SHA256.</summary>
        private const int Iteraciones = 210_000;
        private const int BytesSalt = 16;
        private const int BytesHash = 32;

        /// <summary>Hash nuevo, con salt propio. Formato: PBKDF2$iteraciones$salt$hash.</summary>
        public static string Calcular(string contrasena)
        {
            var salt = RandomNumberGenerator.GetBytes(BytesSalt);
            var hash = Derivar(contrasena, salt, Iteraciones);

            return string.Join(Separador,
                Etiqueta,
                Iteraciones.ToString(),
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash));
        }

        public static VerificacionContrasena Verificar(string contrasena, string hashGuardado)
        {
            if (string.IsNullOrEmpty(hashGuardado))
                return new VerificacionContrasena(false, false);

            if (hashGuardado.StartsWith(Etiqueta + Separador, StringComparison.Ordinal))
                return new VerificacionContrasena(VerificarPbkdf2(contrasena, hashGuardado), false);

            // Formato viejo: SHA1 hexadecimal sin salt.
            return new VerificacionContrasena(VerificarSha1(contrasena, hashGuardado), necesitaRehash: true);
        }

        private static bool VerificarPbkdf2(string contrasena, string hashGuardado)
        {
            var partes = hashGuardado.Split(Separador);
            if (partes.Length != 4 || !int.TryParse(partes[1], out var iteraciones))
                return false;

            byte[] salt, esperado;
            try
            {
                salt = Convert.FromBase64String(partes[2]);
                esperado = Convert.FromBase64String(partes[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            var calculado = Derivar(contrasena, salt, iteraciones, esperado.Length);
            return CryptographicOperations.FixedTimeEquals(calculado, esperado);
        }

        private static bool VerificarSha1(string contrasena, string hashGuardado)
        {
            using var sha1 = SHA1.Create();
            var bytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(contrasena ?? string.Empty));
            var hex = Convert.ToHexString(bytes);

            return CryptographicOperations.FixedTimeEquals(
                Encoding.ASCII.GetBytes(hex),
                Encoding.ASCII.GetBytes(hashGuardado.ToUpperInvariant()));
        }

        private static byte[] Derivar(string contrasena, byte[] salt, int iteraciones, int bytes = BytesHash) =>
            Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(contrasena ?? string.Empty),
                salt,
                iteraciones,
                HashAlgorithmName.SHA256,
                bytes);
    }
}
