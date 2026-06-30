using Meat.Domain.Almacenes;
using Meat.Domain.TiposEspecies;
using Meat.Domain.TiposEstadosHacienda;
using Meat.Domain.Tropas;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.IngresosHaciendas
{
    public class IngresoHaciendaUbicacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid IngresoHaciendaId { get; set; }
        public virtual IngresoHacienda IngresoHacienda { get; set; }

        public Guid? TropaId { get; set; }                 // null en Borrador; se liga al aprobar
        public virtual Tropa Tropa { get; set; }

        public string TipoEspecieId { get; set; }
        public virtual TipoEspecie TipoEspecie { get; set; }

        public Guid AlmacenId { get; set; }                // corral
        public virtual Almacen Almacen { get; set; }

        public int Cantidad { get; set; }                  // UN (animales)
        public double PesoPromedio { get; set; }           // calculado (kg)

        public string EstadoHaciendaId { get; set; }
        public virtual TipoEstadoHacienda EstadoHacienda { get; set; }
    }
}
