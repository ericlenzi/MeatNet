using MediatR;
using System;

namespace Meat.Application.Empresas.DeleteEmpresa
{
    public class DeleteEmpresaRequest : IRequest<DeleteEmpresaResponse>
    {
        public Guid Id { get; set; }
    }
}
