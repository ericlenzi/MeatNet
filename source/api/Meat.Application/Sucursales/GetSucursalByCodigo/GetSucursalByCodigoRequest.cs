using Meat.Application.Shared;
using MediatR;

namespace Meat.Application.Sucursales.GetSucursalByCodigo
{
    public class GetSucursalByCodigoRequest : RequestBase, IRequest<GetSucursalByCodigoResponse>
    {
        public string Codigo { get; set; }
    }
}
