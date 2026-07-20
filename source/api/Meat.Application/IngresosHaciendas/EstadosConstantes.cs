namespace Meat.Application.IngresosHaciendas
{
    /// <summary>Codigos del catalogo TiposEstadosIngresos.</summary>
    public static class EstadosIngreso
    {
        public const string Borrador = "BORRADOR";
        public const string PendienteAprobacion = "PENDIENTE";
        public const string Aprobado = "APROBADO";
        public const string Anulado = "ANULADO";
    }

    /// <summary>Codigos del catalogo TiposEstadosTropas.</summary>
    public static class EstadosTropa
    {
        public const string Recepcionada = "RECEPCIONADA";
        public const string Anulada = "ANULADA";
        public const string Faenada = "FAENADA";
    }

    /// <summary>Codigos del catalogo TiposEstadosHacienda.</summary>
    public static class EstadosHacienda
    {
        public const string EnPie = "EN_PIE";
        public const string Caidos = "CAIDOS";
        public const string Muertos = "MUERTOS";
    }
}
