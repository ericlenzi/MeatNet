using MediatR;
using System.Text.Json.Serialization;

namespace Meat.Application.Sucursales.GetSucursalByCodigo
{
    public class GetSucursalByCodigoRequest : IRequest<GetSucursalByCodigoResponse>
    {
        public string Codigo { get; set; }
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
