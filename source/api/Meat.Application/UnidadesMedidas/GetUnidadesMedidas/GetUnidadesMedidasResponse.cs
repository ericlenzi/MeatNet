using System.Collections.Generic;

namespace Meat.Application.UnidadesMedidas.GetUnidadesMedidas
{
    public class GetUnidadesMedidasResponse
    {
        public IEnumerable<UnidadMedidaItem> Data { get; set; } = new List<UnidadMedidaItem>();
    }

    public class UnidadMedidaItem
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }
}
