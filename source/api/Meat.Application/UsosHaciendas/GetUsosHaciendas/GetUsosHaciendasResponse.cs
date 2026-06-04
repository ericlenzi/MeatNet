using System.Collections.Generic;
using Meat.Domain.UsosHaciendas;

namespace Meat.Application.UsosHaciendas.GetUsosHaciendas
{
    public class GetUsosHaciendasResponse
    {
        public IEnumerable<UsoHacienda> Data { get; set; }
    }
}
