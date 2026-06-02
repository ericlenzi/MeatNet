using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Usuarios.GetUsuarioSucursales
{
    public class GetUsuarioSucursalesRequest : IRequest<GetUsuarioSucursalesResponse>
    {
        public Guid UsuarioId { get; set; }
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
