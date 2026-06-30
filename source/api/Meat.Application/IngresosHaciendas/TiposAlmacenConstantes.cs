namespace Meat.Application.IngresosHaciendas
{
    /// <summary>
    /// Codigos del catalogo TiposAlmacenes para corrales especiales segun el estado de la
    /// hacienda. El stock de faena (En Pie) va a corrales NO especiales; los Caidos/Muertos
    /// van a su corral especial.
    /// </summary>
    public static class TiposAlmacen
    {
        public const string CorralCaidos = "CORRAL_CAIDOS";
        public const string CorralMuertos = "CORRAL_MUERTOS";
    }

    /// <summary>Tolerancia por defecto entre el peso neto del camion y la suma de pesadas.</summary>
    public static class IngresoHaciendaParametros
    {
        public const double ToleranciaPeso = 0.05; // 5%
    }
}
