using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.Sucursales.GetSucursal
{
    public class GetSucursalRequest : RequestBase, IRequest<GetSucursalResponse>
    {
        public Guid Id { get; set; }
    }
}
