using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Almacenes.GetAlmacen
{
    public class GetAlmacenRequest : IRequest<GetAlmacenResponse>
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
