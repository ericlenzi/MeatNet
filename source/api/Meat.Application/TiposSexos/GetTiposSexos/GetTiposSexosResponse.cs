using Meat.Domain.TiposSexos;
using System.Collections.Generic;

namespace Meat.Application.TiposSexos.GetTiposSexos
{
    public class GetTiposSexosResponse
    {
        public IEnumerable<TipoSexo> Data { get; set; }
    }
}
