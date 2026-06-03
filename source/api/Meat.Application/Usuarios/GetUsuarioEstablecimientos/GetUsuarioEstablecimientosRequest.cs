using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Usuarios.GetUsuarioEstablecimientos
{
    public class GetUsuarioEstablecimientosRequest : IRequest<GetUsuarioEstablecimientosResponse>
    {
        public Guid UsuarioId { get; set; }
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
