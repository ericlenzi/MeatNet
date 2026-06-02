using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Clientes.DeleteCliente
{
    public class DeleteClienteRequest : IRequest<DeleteClienteResponse>
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
