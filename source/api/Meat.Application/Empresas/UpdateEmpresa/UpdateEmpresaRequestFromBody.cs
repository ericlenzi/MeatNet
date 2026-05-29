using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Empresas.UpdateEmpresa
{
    public class UpdateEmpresaRequestFromBody
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
        public bool Activo { get; set; }
    }
}
