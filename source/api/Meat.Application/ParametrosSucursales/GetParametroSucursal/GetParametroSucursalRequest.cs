using MediatR;
using System;

namespace Meat.Application.ParametrosSucursales.GetParametroSucursal
{
    public class GetParametroSucursalRequest : IRequest<GetParametroSucursalResponse>
    {
        public Guid Id { get; set; }
    }
}
