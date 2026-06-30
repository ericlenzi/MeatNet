using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Almacenes.DeleteAlmacen
{
    public class DeleteAlmacenRequest : IRequest<DeleteAlmacenResponse>
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
