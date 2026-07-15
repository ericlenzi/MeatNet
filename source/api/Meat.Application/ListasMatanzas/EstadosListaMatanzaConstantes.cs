namespace Meat.Application.ListasMatanzas
{
    /// <summary>Codigos del catalogo TiposEstadosListasMatanzas.</summary>
    public static class EstadosListaMatanza
    {
        public const string Borrador = "BORRADOR";
        public const string Confirmada = "CONFIRMADA";
        public const string EnEjecucion = "EN_EJECUCION";
        public const string Finalizada = "FINALIZADA";
        public const string Anulada = "ANULADA";
    }

    /// <summary>Tipos de movimiento del historial de la Lista de Matanza.</summary>
    public static class TiposMovimientoLM
    {
        public const string AltaTropa = "ALTA_TROPA";
        public const string BajaTropa = "BAJA_TROPA";
        public const string Incremento = "INCREMENTO";
        public const string Decremento = "DECREMENTO";
        public const string CambioSecuencia = "CAMBIO_SECUENCIA";
        public const string Division = "DIVISION";
        public const string Fusion = "FUSION";
        public const string FaenaEmergencia = "FAENA_EMERGENCIA";
        public const string Confirmacion = "CONFIRMACION";
        public const string Desconfirmacion = "DESCONFIRMACION";
        public const string Cancelacion = "CANCELACION";
        public const string Inicio = "INICIO";
        public const string Finalizacion = "FINALIZACION";
        public const string Liberacion = "LIBERACION";
    }
}
