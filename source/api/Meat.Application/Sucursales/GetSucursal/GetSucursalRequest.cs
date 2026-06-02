using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Sucursales.GetSucursal
{
    public class GetSucursalRequest : IRequest<GetSucursalResponse>
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
