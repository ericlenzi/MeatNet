using System.Collections.Generic;

namespace Meat.Application.TipificacionesOficiales.GetTipificacionesOficiales
{
    public class GetTipificacionesOficialesResponse
    {
        public IEnumerable<TipificacionOficialItem> Data { get; set; } = new List<TipificacionOficialItem>();
    }

    public class TipificacionOficialItem
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
    }
}
