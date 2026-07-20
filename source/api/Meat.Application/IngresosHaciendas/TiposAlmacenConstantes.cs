namespace Meat.Application.IngresosHaciendas
{
    /// <summary>
    /// Codigos del catalogo TiposAlmacenes para corrales especiales segun el estado de la
    /// hacienda. El stock de faena (En Pie) va a corrales NO especiales; los Caidos/Muertos
    /// van a su corral especial.
    /// </summary>
    public static class TiposAlmacen
    {
        public const string CorralComun = "CORRAL_COMUN";
        public const string CorralCaidos = "CORRAL_CAIDOS";
        public const string CorralMuertos = "CORRAL_MUERTOS";
        public const string CamaraFaena = "CAMARA_FAENA";
    }

    /// <summary>
    /// Familia del tipo de almacen: agrupa los tipos por proposito. Un CORRAL es de
    /// recepcion (hacienda en pie); una CAMARA es destino de faena (media res).
    /// </summary>
    public static class FamiliaAlmacen
    {
        public const string Corral = "CORRAL";
        public const string Camara = "CAMARA";
    }
}
