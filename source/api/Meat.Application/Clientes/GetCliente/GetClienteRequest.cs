using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Clientes.GetCliente
{
    public class GetClienteRequest : IRequest<GetClienteResponse>
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
