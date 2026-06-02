using MediatR;
using Meat.Application.Shared;
using System;

namespace Meat.Application.Establecimientos.GetEstablecimientos
{

    public class GetEstablecimientosRequest : RequestListBase, IRequest<GetEstablecimientosResponse>
    {
        public bool? Estado { get; set; }
        public Guid? SucursalId { get; set; }
    }
}
