using System.Collections.Generic;

namespace Meat.Application.DestinosComerciales.GetDestinosComerciales
{
    public class GetDestinosComercialesResponse
    {
        public IEnumerable<DestinoComercialItem> Data { get; set; } = new List<DestinoComercialItem>();
    }

    public class DestinoComercialItem
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }
}
