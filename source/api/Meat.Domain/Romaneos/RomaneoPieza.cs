using Meat.Domain.Almacenes;
using Meat.Domain.Tipificaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.Romaneos
{
    /// <summary>
    /// Pieza fisica pesada de un Romaneo. PORCINO: 1 pieza (Letra null, RES).
    /// VACUNO: 2 piezas (MEDIA RES, Letra "A" / "B"). Peso es la cache desnormalizada
    /// de la medicion PESO (canonico para elegir la Tipificacion por rango y para KG).
    /// </summary>
    public class RomaneoPieza
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid RomaneoId { get; set; }
        public virtual Romaneo Romaneo { get; set; }

        public string Letra { get; set; }                      // "A" / "B"; null para porcino

        public Guid AlmacenDestinoId { get; set; }             // camara destino de esta pieza: default del renglon (LM), editable y obligatoria
        public virtual Almacen AlmacenDestino { get; set; }

        public string TipificacionId { get; set; }
        public virtual Tipificacion Tipificacion { get; set; }

        public double Peso { get; set; }                       // cache de la medicion PESO

        public virtual ICollection<RomaneoPiezaMedicion> Mediciones { get; set; }
    }
}
