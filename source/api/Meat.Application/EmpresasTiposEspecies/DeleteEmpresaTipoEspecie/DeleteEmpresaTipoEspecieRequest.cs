using MediatR;
using System;

namespace Meat.Application.EmpresasTiposEspecies.DeleteEmpresaTipoEspecie
{
    public class DeleteEmpresaTipoEspecieRequest : IRequest<DeleteEmpresaTipoEspecieResponse>
    {
        public Guid Id { get; set; }
    }
}
