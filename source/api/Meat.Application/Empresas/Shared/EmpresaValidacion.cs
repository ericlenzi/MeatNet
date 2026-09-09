using Meat.Application.Shared;
using System;
using System.Text.RegularExpressions;

namespace Meat.Application.Empresas.Shared
{
    /// <summary>
    /// Validaciones de la identidad visual de la empresa: el color que pinta su panel y el
    /// logo, que viaja como data URI porque se guarda en la propia base (no hay almacenamiento
    /// de archivos en el sistema).
    /// </summary>
    public static class EmpresaValidacion
    {
        /// <summary>Tope del logo ya decodificado. Un logo es chico; el limite corta subidas
        /// de fotos por error, que engordarian cada fila de Empresas.</summary>
        public const int LogoMaximoBytes = 200 * 1024;

        private static readonly Regex ColorHex = new Regex(
            "^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6})$", RegexOptions.Compiled);

        // Solo mapa de bits: un SVG puede traer scripts adentro y no hace falta para un logo.
        private static readonly Regex LogoDataUri = new Regex(
            "^data:image/(png|jpeg|jpg|gif|webp);base64,[A-Za-z0-9+/]+={0,2}$",
            RegexOptions.Compiled);

        public static void ValidarColor(string color)
        {
            if (string.IsNullOrWhiteSpace(color))
                return;

            if (!ColorHex.IsMatch(color.Trim()))
                throw new ValidationException("El color debe ser un hexadecimal como #1B4F72.");
        }

        public static void ValidarLogo(string logo)
        {
            if (string.IsNullOrWhiteSpace(logo))
                return;

            if (!LogoDataUri.IsMatch(logo.Trim()))
                throw new ValidationException(
                    "El logo debe ser una imagen PNG, JPG, GIF o WEBP.");

            var base64 = logo.Substring(logo.IndexOf(',') + 1);
            var relleno = base64.EndsWith("==") ? 2 : base64.EndsWith("=") ? 1 : 0;
            var bytes = (base64.Length / 4L) * 3 - relleno;

            if (bytes > LogoMaximoBytes)
                throw new ValidationException(
                    $"El logo no puede superar los {LogoMaximoBytes / 1024} KB.");
        }
    }
}
