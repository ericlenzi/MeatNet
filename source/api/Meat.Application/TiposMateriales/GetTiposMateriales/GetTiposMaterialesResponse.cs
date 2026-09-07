using System.Collections.Generic;

namespace Meat.Application.TiposMateriales.GetTiposMateriales
{
    public class GetTiposMaterialesResponse
    {
        public IEnumerable<TipoMaterialItem> Data { get; set; } = new List<TipoMaterialItem>();
    }

    public class TipoMaterialItem
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }
}
