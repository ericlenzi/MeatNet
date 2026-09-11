using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.RendimientosSubproductos.GetRendimientosSubproductos
{
    public class GetRendimientosSubproductosRequest : RequestListBase, IRequest<GetRendimientosSubproductosResponse>
    {
        public string EspecieId { get; set; }
        public bool? Estado { get; set; }
    }
}
