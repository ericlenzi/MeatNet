using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.UnidadesFaenas.GetUnidadesFaenas
{
    public class GetUnidadesFaenasRequest : RequestListBase, IRequest<GetUnidadesFaenasResponse>
    {
        public bool? Estado { get; set; }
        public string EspecieId { get; set; }
    }
}
