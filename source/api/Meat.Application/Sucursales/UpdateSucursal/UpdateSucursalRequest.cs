using MediatR;
using System;

namespace Meat.Application.Sucursales.UpdateSucursal
{

    public class UpdateSucursalRequest : IRequest<UpdateSucursalResponse>
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public Guid EmpresaId { get; set; }
        public bool Activa { get; set; }
    }
}
