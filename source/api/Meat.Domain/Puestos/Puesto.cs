using Meat.Domain.Establecimientos;
using Meat.Domain.TiposPuestos;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.Puestos
{
    public class Puesto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public string CodigoPuesto { get; set; }
        public string Nombre { get; set; }
        public string Erp_Codigo { get; set; }
        public Guid EstablecimientoId { get; set; }
        public virtual Establecimiento Establecimiento { get; set; }
        public string TipoPuestoId { get; set; }
        public virtual TipoPuesto TipoPuesto { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public bool Activo { get; set; }
    }
}
