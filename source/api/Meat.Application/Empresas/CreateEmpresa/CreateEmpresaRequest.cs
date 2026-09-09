using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Empresas.CreateEmpresa
{
    public class CreateEmpresaRequest : IRequest<CreateEmpresaResponse>
    {
        /// <summary>Codigo de negocio de la empresa: es su clave primaria.</summary>
        [Required]
        public string Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string TipoEmpresaId { get; set; }

        public string NumeroCuit { get; set; }
        public string NumeroIngresosBrutos { get; set; }
        public string NumeroInscripcionRuca { get; set; }
        public string CodigoActividad { get; set; }
        public string ERP_Codigo { get; set; }
        /// <summary>Color identitario; pinta el panel del dashboard de la empresa.</summary>
        public string Color { get; set; }
        /// <summary>Logo como data URI base64. Se guarda en la base: no hay store de archivos.</summary>
        public string Logo { get; set; }
    }
}
