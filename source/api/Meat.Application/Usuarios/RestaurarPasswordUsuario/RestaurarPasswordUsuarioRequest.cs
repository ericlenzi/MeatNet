using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Usuarios.RestaurarPasswordUsuario
{
    public class RestaurarPasswordUsuarioRequest : IRequest<RestaurarPasswordUsuarioResponse>
    {
        public Guid UsuarioId { get; set; }
        [JsonIgnore]
        public string EmpresaId { get; set; }
    }
}
