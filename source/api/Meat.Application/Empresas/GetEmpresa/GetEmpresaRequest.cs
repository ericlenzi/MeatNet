using MediatR;
using System;

namespace Meat.Application.Empresas.GetEmpresa
{
    public class GetEmpresaRequest : IRequest<GetEmpresaResponse>
    {
        public string Id { get; set; }
    }
}
