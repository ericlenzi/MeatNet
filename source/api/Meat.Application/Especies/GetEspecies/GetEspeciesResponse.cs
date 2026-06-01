using System.Collections.Generic;
using Meat.Domain.Especies;

namespace Meat.Application.Especies.GetEspecies
{
    public class GetEspeciesResponse
    {
        public IEnumerable<Especie> Data { get; set; }
    }
}
