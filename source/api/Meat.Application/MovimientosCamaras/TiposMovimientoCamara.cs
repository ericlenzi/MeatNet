namespace Meat.Application.MovimientosCamaras
{
    /// <summary>
    /// Tipos de movimiento del log de existencia de camara. Deben existir en el
    /// catalogo TiposMovimientosCamaras (sembrado por migracion).
    /// </summary>
    public static class TiposMovimientoCamara
    {
        // Nace la existencia: la pieza liberada entra a camara tal cual (sin despiece).
        public const string Ingreso = "INGRESO";

        // Cuarteo: baja el material origen (media res) y da de alta los destinos (cuartos).
        // Ambos comparten TransformacionId.
        public const string TransformacionBaja = "TRANSF_BAJA";
        public const string TransformacionAlta = "TRANSF_ALTA";

        // Salida hacia Ciclo II (Despostada). Lo usara ese modulo; aqui solo se declara.
        public const string Egreso = "EGRESO";
    }
}
