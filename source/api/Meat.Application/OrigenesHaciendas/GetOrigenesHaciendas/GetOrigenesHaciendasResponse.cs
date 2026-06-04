using System.Collections.Generic;
using Meat.Domain.OrigenesHaciendas;

namespace Meat.Application.OrigenesHaciendas.GetOrigenesHaciendas
{
    public class GetOrigenesHaciendasResponse
    {
        public IEnumerable<OrigenHacienda> Data { get; set; }
    }
}
