using Meat.Domain.TiposEspecies;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.IngresosHaciendas
{
    public class IngresoHaciendaPesada
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid IngresoHaciendaId { get; set; }
        public virtual IngresoHacienda IngresoHacienda { get; set; }

        public string TipoEspecieId { get; set; }
        public virtual TipoEspecie TipoEspecie { get; set; }

        public double PesoIngreso { get; set; }            // kg
        public string UnidadMedida { get; set; }           // "KG"
        public string IdPesada { get; set; }               // numero de ticket de la balanza (string)
    }
}
