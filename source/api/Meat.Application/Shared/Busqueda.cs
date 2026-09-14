namespace Meat.Application.Shared
{
    /// <summary>
    /// Filtros de texto de los listados. PostgreSQL distingue mayusculas en LIKE, asi que los
    /// handlers buscan con EF.Functions.ILike(columna, Busqueda.Contiene(request.Filter)) en vez
    /// de columna.Contains(filtro), que en SQL Server no las distinguia por la collation.
    /// </summary>
    public static class Busqueda
    {
        /// <summary>
        /// Patron ILIKE "contiene": escapa los comodines que tipee el usuario (% y _ se buscan
        /// literales) con la barra invertida, que es el escape por defecto de PostgreSQL.
        /// </summary>
        public static string Contiene(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            var escapado = texto
                .Replace("\\", "\\\\")
                .Replace("%", "\\%")
                .Replace("_", "\\_");

            return "%" + escapado + "%";
        }
    }
}
