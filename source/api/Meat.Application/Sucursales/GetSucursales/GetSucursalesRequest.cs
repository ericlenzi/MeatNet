using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.Sucursales.GetSucursales
{
    public class GetSucursalesRequest : RequestListBase, IRequest<GetSucursalesResponse>
    {
        public bool? Estado { get; set; }
    }
}
