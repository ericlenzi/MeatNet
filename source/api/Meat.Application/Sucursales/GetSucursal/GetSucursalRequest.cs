using MediatR;
using System;

namespace Meat.Application.Sucursales.GetSucursal
{
    public class GetSucursalRequest : IRequest<GetSucursalResponse>
    {
        public Guid Id { get; set; }
    }
}
