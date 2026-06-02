using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.Especies.GetEspecies
{
    public class GetEspeciesRequest : RequestListBase, IRequest<GetEspeciesResponse>
    {
        public bool? Estado { get; set; }
    }
}
