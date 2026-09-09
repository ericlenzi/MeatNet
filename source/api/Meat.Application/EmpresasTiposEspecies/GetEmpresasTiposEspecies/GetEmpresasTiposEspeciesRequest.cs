using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.EmpresasTiposEspecies.GetEmpresasTiposEspecies
{
    public class GetEmpresasTiposEspeciesRequest : RequestListBase, IRequest<GetEmpresasTiposEspeciesResponse>
    {
        /// <summary>Estado efectivo: cuenta el Activo de la empresa y el del catalogo.</summary>
        public bool? Estado { get; set; }
        public string EspecieId { get; set; }
    }
}
