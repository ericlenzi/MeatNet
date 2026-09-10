using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.TiposContusiones.GetTiposContusiones
{
    public class GetTiposContusionesRequest : RequestListBase, IRequest<GetTiposContusionesResponse>
    {
        public bool? Estado { get; set; }
        public string EspecieId { get; set; }
    }
}
