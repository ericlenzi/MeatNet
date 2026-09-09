using Meat.Application.Shared;
using System.Collections.Generic;

namespace Meat.Application.Empresas.GetEmpresas
{
    public class GetEmpresasResponse : ResponseListBase<IEnumerable<EmpresaItem>>
    {
    }

    /// <summary>
    /// El listado no incluye el Logo a proposito: es un data URI de hasta 200 KB y traerlo
    /// por cada fila haria pesada la grilla. El logo se pide al abrir la empresa.
    /// </summary>
    public class EmpresaItem
    {
        /// <summary>Codigo de negocio de la empresa: es su clave primaria.</summary>
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string TipoEmpresaId { get; set; }
        public string NumeroCuit { get; set; }
        public string NumeroIngresosBrutos { get; set; }
        public string NumeroInscripcionRuca { get; set; }
        public string CodigoActividad { get; set; }
        public string ERP_Codigo { get; set; }
        public string Color { get; set; }
        public bool Activo { get; set; }
    }
}
