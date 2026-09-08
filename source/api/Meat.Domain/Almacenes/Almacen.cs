using Meat.Domain.Establecimientos;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Meat.Domain.TiposAlmacenes;
using Meat.Domain.Empresas;
using Meat.Domain.Shared;

namespace Meat.Domain.Almacenes
{
    public class Almacen : ITenantScoped
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public string CodigoAlmacen { get; set; }
        public string Nombre { get; set; }
        /// <summary>Capacidad de la ubicacion. Unidad segun la familia (cabezas en corral, ganchos/reses en camara).</summary>
        public int Capacidad { get; set; }
        public string TipoAlmacenId { get; set; }
        public TipoAlmacen TipoAlmacen { get; set; }
        public Guid EstablecimientoId { get; set; }
        public virtual Establecimiento Establecimiento { get; set; }
        public bool Activo { get; set; }
        public string ERP_Codigo { get; set; }
        public DateTime FechaActualizacion { get; set; }

        /// <summary>Empresa (tenant) duena del registro. La asigna el MeatContext en el alta.</summary>
        public string EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}