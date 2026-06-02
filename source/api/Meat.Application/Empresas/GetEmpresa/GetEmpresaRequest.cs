using MediatR;
using System;

namespace Meat.Application.Empresas.GetEmpresa
{
    public class GetEmpresaRequest : IRequest<GetEmpresaResponse>
    {
        public Guid Id { get; set; }
    }
}
