using MediatR;
using System;

namespace Meat.Application.Usuarios.AddUsuarioSucursal
{
    public class AddUsuarioSucursalRequest : IRequest<AddUsuarioSucursalResponse>
    {
        public Guid UsuarioId { get; set; }
        public Guid SucursalId { get; set; }
        public bool EsMain { get; set; }
    }
}
