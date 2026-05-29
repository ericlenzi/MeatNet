using MediatR;
using System;

namespace Meat.Application.ParametrosSucursales.UpdateParametroSucursal
{
    public class UpdateParametroSucursalRequest : IRequest<UpdateParametroSucursalResponse>
    {
        public Guid Id { get; set; }
        public Guid SucursalId { get; set; }
        public Guid ParametroId { get; set; }
        public string Valor { get; set; }
    }
}
