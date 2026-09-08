using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.Sucursales.DeleteSucursal
{
    public class DeleteSucursalRequest : RequestBase, IRequest<DeleteSucursalResponse>
    {
        public Guid Id { get; set; }
    }
}
