using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.Establecimientos.GetEstablecimientos
{

    public class GetEstablecimientosRequest : RequestListBase, IRequest<GetEstablecimientosResponse>
    {
        public bool? Estado { get; set; }
    }
}
