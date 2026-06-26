using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.TiposEspecies.GetTiposEspecies
{
    public class GetTiposEspeciesRequest : RequestListBase, IRequest<GetTiposEspeciesResponse>
    {
        public bool? Estado { get; set; }
        public string EspecieId { get; set; }
    }
}
