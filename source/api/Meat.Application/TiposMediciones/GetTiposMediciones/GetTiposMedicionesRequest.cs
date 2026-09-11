using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.TiposMediciones.GetTiposMediciones
{
    public class GetTiposMedicionesRequest : RequestListBase, IRequest<GetTiposMedicionesResponse>
    {
        public bool? Estado { get; set; }
    }
}
