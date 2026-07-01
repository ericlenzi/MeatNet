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
}
