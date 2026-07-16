namespace Meat.Application.Tropas
{
    /// <summary>
    /// Tipos de movimiento del historial de trazabilidad de la Tropa.
    /// Extender a medida que los nuevos procesos (planificacion, faena, romaneo)
    /// generen eventos sobre la tropa.
    /// </summary>
    public static class TiposMovimientoTropa
    {
        public const string Recepcion = "RECEPCION";
        public const string Ubicacion = "UBICACION";
        public const string Anulacion = "ANULACION";
        // La Planificacion se integra por merge desde ListaMatanzaMovimiento (no se
        // duplica aqui). Futuros (Monitor/Evaluacion): Faena, Cierre, etc.
    }
}
