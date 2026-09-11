using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.TiposPuestos.GetTiposPuestos
{
    public class GetTiposPuestosRequest : RequestListBase, IRequest<GetTiposPuestosResponse>
    {
        public bool? Estado { get; set; }
    }
}
