using MediatR;
using Meat.Application.Shared;
using System;

namespace Meat.Application.ParametrosSucursales.GetParametrosSucursales
{
    public class GetParametrosSucursalesRequest : RequestListBase, IRequest<GetParametrosSucursalesResponse>
    {
        public Guid SucursalId { get; set; }
    }
}
