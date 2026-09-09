using System;

namespace Meat.Application.Tipificaciones.GetTipificacion
{
    public class GetTipificacionResponse
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string EspecieId { get; set; }
        public string EspecieNombre { get; set; }
        public string TipoEspecieId { get; set; }
        public string TipoEspecieNombre { get; set; }
        public Guid? UnidadFaenaId { get; set; }
        public string UnidadFaenaNombre { get; set; }
        public Guid? DestinoComercialId { get; set; }
        public string DestinoComercialNombre { get; set; }
        public string TipificacionOficialId { get; set; }
        public string TipificacionOficialNombre { get; set; }
        public double PesoDesde { get; set; }
        public double PesoHasta { get; set; }
        public string UnidadMedidaId { get; set; }
        public string UnidadMedidaNombre { get; set; }
        public Guid? MaterialId { get; set; }
        public string MaterialCodigo { get; set; }
        public string MaterialNombre { get; set; }
        public int Puntos { get; set; }
        public bool Activo { get; set; }
    }
}
