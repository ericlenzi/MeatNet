using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Empresas.CreateEmpresa
{
    public class CreateEmpresaRequest : IRequest<CreateEmpresaResponse>
    {
        [Required]
        public string CodigoEmpresa { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string TipoEmpresaId { get; set; }

        public string NumeroCuit { get; set; }
        public string NumeroIngresosBrutos { get; set; }
        public string NumeroInscripcionRuca { get; set; }
        public string CodigoActividad { get; set; }
        public string ERP_Codigo { get; set; }
    }
}
