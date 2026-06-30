using Meat.Domain.Establecimientos;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Meat.Domain.TiposAlmacenes;

namespace Meat.Domain.Almacenes
{
    public class Almacen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public string CodigoAlmacen { get; set; }
        public string Nombre { get; set; }
        public int CantidadAnimales { get; set; }
        public string TipoAlmacenId { get; set; }
        public TipoAlmacen TipoAlmacen { get; set; }
        public Guid EstablecimientoId { get; set; }
        public virtual Establecimiento Establecimiento { get; set; }
        public bool Activo { get; set; }
        public string ERP_Codigo { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}