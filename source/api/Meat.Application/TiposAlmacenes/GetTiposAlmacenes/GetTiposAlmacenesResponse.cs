using System.Collections.Generic;

namespace Meat.Application.TiposAlmacenes.GetTiposAlmacenes
{
    public class GetTiposAlmacenesResponse
    {
        public IEnumerable<TipoAlmacenItem> Data { get; set; } = new List<TipoAlmacenItem>();
    }

    public class TipoAlmacenItem
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Familia { get; set; }
    }
}
