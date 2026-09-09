using MediatR;
using System;

namespace Meat.Application.EmpresasTiposEspecies.GetEmpresaTipoEspecie
{
    public class GetEmpresaTipoEspecieRequest : IRequest<GetEmpresaTipoEspecieResponse>
    {
        public Guid Id { get; set; }
    }
}
