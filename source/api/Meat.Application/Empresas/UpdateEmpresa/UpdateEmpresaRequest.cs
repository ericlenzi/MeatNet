using MediatR;
using System;

namespace Meat.Application.Empresas.UpdateEmpresa
{
    public class UpdateEmpresaRequest : IRequest<UpdateEmpresaResponse>
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
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
        public bool Activo { get; set; }
    }
}
