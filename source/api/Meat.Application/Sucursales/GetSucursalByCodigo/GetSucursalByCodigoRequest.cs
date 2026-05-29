using MediatR;

namespace Meat.Application.Sucursales.GetSucursalByCodigo
{
    public class GetSucursalByCodigoRequest : IRequest<GetSucursalByCodigoResponse>
    {
        public string Codigo { get; set; }
    }
}
