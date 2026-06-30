using System.Collections.Generic;

namespace Meat.Application.Provincias.GetProvincias
{
    public class GetProvinciasResponse
    {
        public IEnumerable<ProvinciaItem> Data { get; set; } = new List<ProvinciaItem>();
    }

    public class ProvinciaItem
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
