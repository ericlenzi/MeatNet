using Meat.Domain.Especies;
using Meat.Domain.Establecimientos;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.EstablecimientosEspecies
{
    public class EstablecimientoEspecie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public Guid EstablecimientoId { get; set; }
        public virtual Establecimiento Establecimiento { get; set; }
        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
