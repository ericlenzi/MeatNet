using Meat.Domain.Establecimientos;
using Meat.Domain.Sucursales;
using Meat.Domain.TiposEmpresas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.Empresas
{
    /// <summary>
    /// Raiz del tenant: cada registro es una empresa aislada del resto.
    /// La PK es el codigo de negocio (ex CodigoEmpresa), no un Guid: es el mismo valor que
    /// viaja en el claim del JWT, asi las queries filtran por EmpresaId sin joinear a Empresas.
    /// </summary>
    public class Empresa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string TipoEmpresaId { get; set; }
        public TipoEmpresa TipoEmpresa { get; set; }
        public string NumeroCuit { get; set; }
        public string NumeroIngresosBrutos { get; set; }
        public string NumeroInscripcionRuca { get; set; }
        public string CodigoActividad { get; set; }
        /// <summary>Color identitario de la empresa; pinta el panel del dashboard.</summary>
        public string Color { get; set; }
        /// <summary>Logo como data URI base64 (data:image/...;base64,...). Se muestra en el dashboard.</summary>
        public string Logo { get; set; }
        public bool Activo { get; set; }
        public string ERP_Codigo { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public virtual IEnumerable<Sucursal> Sucursales { get; set; }
        public virtual IEnumerable<Establecimiento> Establecimientos { get; set; }
    }
}
