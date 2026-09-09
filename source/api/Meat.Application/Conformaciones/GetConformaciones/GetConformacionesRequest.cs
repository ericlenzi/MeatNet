using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.Conformaciones.GetConformaciones
{
    public class GetConformacionesRequest : RequestListBase, IRequest<GetConformacionesResponse>
    {
        public bool? Estado { get; set; }
        public string EspecieId { get; set; }
    }
}
