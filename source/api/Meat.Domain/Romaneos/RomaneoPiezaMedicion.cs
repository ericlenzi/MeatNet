using Meat.Domain.TiposMediciones;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.Romaneos
{
    /// <summary>
    /// Medicion capturada de una pieza, tipada por el catalogo TiposMediciones.
    /// En el MVP (Fase 2) la unica medicion es PESO; la tabla queda para extender a
    /// Fase 2b (mas mediciones) sin cambiar el esquema.
    /// </summary>
    public class RomaneoPiezaMedicion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid RomaneoPiezaId { get; set; }
        public virtual RomaneoPieza RomaneoPieza { get; set; }

        public string TipoMedicionId { get; set; }
        public virtual TipoMedicion TipoMedicion { get; set; }

        public double Valor { get; set; }
    }
}
