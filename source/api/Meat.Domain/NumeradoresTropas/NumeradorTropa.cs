using Meat.Domain.ClientesEstablecimientos;
using Meat.Domain.Especies;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.NumeradoresTropas
{
    public class NumeradorTropa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid ClienteEstablecimientoId { get; set; }
        public virtual ClienteEstablecimiento ClienteEstablecimiento { get; set; }

        public string EspecieCodigo { get; set; }
        public virtual Especie Especie { get; set; }

        public long UltimoNumeroTropa { get; set; }
    }
}
