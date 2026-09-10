using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.MotivosDecomisos.GetMotivosDecomisos
{
    public class GetMotivosDecomisosRequest : RequestListBase, IRequest<GetMotivosDecomisosResponse>
    {
        public bool? Estado { get; set; }
        public string EspecieId { get; set; }
    }
}
