using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.EmpresasTiposEspecies.CreateEmpresaTipoEspecie
{
    public class CreateEmpresaTipoEspecieRequest : IRequest<CreateEmpresaTipoEspecieResponse>
    {
        [Required]
        public string TipoEspecieId { get; set; }

        /// <summary>
        /// Peso teorico propio de la empresa. Si no viene, se toma el de referencia del catalogo.
        /// </summary>
        public double? PesoTeorico { get; set; }
        public string ERP_Codigo { get; set; }
    }
}
