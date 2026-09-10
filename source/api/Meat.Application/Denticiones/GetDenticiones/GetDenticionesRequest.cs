using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.Denticiones.GetDenticiones
{
    public class GetDenticionesRequest : RequestListBase, IRequest<GetDenticionesResponse>
    {
        public bool? Estado { get; set; }
        public string EspecieId { get; set; }
    }
}
