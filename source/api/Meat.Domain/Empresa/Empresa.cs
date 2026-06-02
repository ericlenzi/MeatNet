using Meat.Domain.Enums;
using Meat.Domain.Sucursales;
using Meat.Domain.TiposEmpresas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.Empresas
{
    public class Empresa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public string CodigoEmpresa { get; set; }
        public string Nombre { get; set; }
        public string TipoEmpresaId { get; set; }
        public TipoEmpresa TipoEmpresa { get; set; }
        public string NumeroCuit { get; set; }
        public string NumeroIngresosBrutos { get; set; }
        public string NumeroInscripcionRuca { get; set; }
        public string CodigoActividad { get; set; }
        public bool Activo { get; set; }
        public string ERP_Codigo { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public Guid? EmpresaId { get; set; }
        [ForeignKey("EmpresaId")]
        public virtual Empresa EmpresaPadre { get; set; }
        public virtual IEnumerable<Sucursal> Sucursales { get; set; }
    }
}