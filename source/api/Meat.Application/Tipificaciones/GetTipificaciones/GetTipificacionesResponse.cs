using Meat.Application.Shared;
using System;
using System.Collections.Generic;

namespace Meat.Application.Tipificaciones.GetTipificaciones
{
    public class GetTipificacionesResponse : ResponseListBase<IEnumerable<TipificacionItem>>
    {
    }

    public class TipificacionItem
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string EspecieId { get; set; }
        public string EspecieNombre { get; set; }
        public string TipoEspecieId { get; set; }
        public string TipoEspecieNombre { get; set; }
        public string UnidadFaenaId { get; set; }
        public string UnidadFaenaNombre { get; set; }
        public string DestinoComercialId { get; set; }
        public string DestinoComercialNombre { get; set; }
        public string TipificacionOficialId { get; set; }
        public string TipificacionOficialNombre { get; set; }
        public double PesoDesde { get; set; }
        public double PesoHasta { get; set; }
        public string UnidadMedidaId { get; set; }
        public int Puntos { get; set; }
        public bool Activo { get; set; }
    }
}
