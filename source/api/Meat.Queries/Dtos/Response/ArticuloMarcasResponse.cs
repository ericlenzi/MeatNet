using System.Collections.Generic;

namespace Meat.Queries.Dtos.Response
{
    public class ArticuloMarcasResponse
    {
        public IEnumerable<ArticulosMarcasDto> Marcas { get; set; }
    }
}
