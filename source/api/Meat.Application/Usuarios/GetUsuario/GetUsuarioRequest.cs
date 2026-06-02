using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Usuarios.GetUsuario
{
    public class GetUsuarioRequest : IRequest<GetUsuarioResponse>
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
