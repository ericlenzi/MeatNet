using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.Empresas.GetEmpresas
{
    public class GetEmpresasRequest : RequestListBase, IRequest<GetEmpresasResponse>
    {
        public bool? Estado { get; set; }
    }
}
