using System;

namespace Meat.Application.Almacenes.GetAlmacen
{
    public class GetAlmacenResponse
    {
        public Guid Id { get; set; }
        public string CodigoAlmacen { get; set; }
        public string Nombre { get; set; }
        public int CantidadAnimales { get; set; }
        public string TipoAlmacenId { get; set; }
        public string TipoAlmacenNombre { get; set; }
        public Guid EstablecimientoId { get; set; }
        public string EstablecimientoNombre { get; set; }
        public bool Activo { get; set; }
        public string ERP_Codigo { get; set; }
    }
}
