using System;

namespace Meat.Application.Empresas.GetEmpresa
{
    public class GetEmpresaResponse
    {
        public Guid Id { get; set; }
        public string CodigoEmpresa { get; set; }
        public string Nombre { get; set; }
        public string TipoEmpresaId { get; set; }
        public string NumeroCuit { get; set; }
        public string NumeroIngresosBrutos { get; set; }
        public string NumeroInscripcionRuca { get; set; }
        public string CodigoActividad { get; set; }
        public string ERP_Codigo { get; set; }
        public bool Activo { get; set; }
    }
}
