using MediatR;
using System;

namespace Meat.Application.EmpresasTiposEspecies.UpdateEmpresaTipoEspecie
{
    public class UpdateEmpresaTipoEspecieRequest : IRequest<UpdateEmpresaTipoEspecieResponse>
    {
        public Guid Id { get; set; }
        public double PesoTeorico { get; set; }
        public string ERP_Codigo { get; set; }
        public bool Activo { get; set; }
    }
}
