using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.Clientes.GetClientes
{
    public class GetClientesRequest : RequestListBase, IRequest<GetClientesResponse>
    {
        public bool? Estado { get; set; }
    }
}
