using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Sucursales.DeleteSucursal
{
    public class DeleteSucursalRequest : IRequest<DeleteSucursalResponse>
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
