using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.Materiales.GetMateriales
{
    public class GetMaterialesRequest : RequestListBase, IRequest<GetMaterialesResponse>
    {
        public bool? Estado { get; set; }
        public string TipoMaterialId { get; set; }
    }
}
