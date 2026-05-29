using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.ParametrosSucursales.GetParametroSucursalByCodigo
{
    public class GetParametroSucursalByCodigoRequest : RequestListBase, IRequest<GetParametroSucursalByCodigoResponse>
    {
        public string Codigo { get; set; }
    }
}
