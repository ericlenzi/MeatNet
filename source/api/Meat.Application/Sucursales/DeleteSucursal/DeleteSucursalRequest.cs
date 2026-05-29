using MediatR;
using System;

namespace Meat.Application.Sucursales.DeleteSucursal
{
    public class DeleteSucursalRequest : IRequest<DeleteSucursalResponse>
    {
        public Guid Id { get; set; }
    }
}
