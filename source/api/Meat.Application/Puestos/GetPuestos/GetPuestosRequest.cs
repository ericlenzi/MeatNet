using MediatR;
using Meat.Application.Shared;
using System;

namespace Meat.Application.Puestos.GetPuestos
{
    public class GetPuestosRequest : RequestListBase, IRequest<GetPuestosResponse>
    {
        public Guid SucursalId { get; set; }
    }
}
