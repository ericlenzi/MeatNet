using Meat.Domain.Especies;
using Meat.Domain.ListasMatanzas;
using Meat.Domain.Tropas;
using Meat.Domain.UnidadesFaenas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.Romaneos
{
    /// <summary>
    /// Romaneo de un animal faenado (Ciclo I paso 3). Es la unidad que consume stock:
    /// 1 romaneo = 1 animal = CantidadFaenada += 1 en el renglon de la LM, sin importar
    /// cuantas piezas tenga (VACUNO = 2 medias reses A/B; PORCINO = 1 res). Cuelga de la
    /// LM EN_EJECUCION (jornada) y del renglon elegido.
    /// </summary>
    public class Romaneo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid ListaMatanzaId { get; set; }               // jornada (LM en ejecucion)
        public virtual ListaMatanza ListaMatanza { get; set; }

        public Guid ListaMatanzaDetalleId { get; set; }        // renglon elegido (Tropa+Corral+TipoEspecie)
        public virtual ListaMatanzaDetalle ListaMatanzaDetalle { get; set; }

        public Guid TropaId { get; set; }                      // denormalizado del renglon (trazabilidad)
        public virtual Tropa Tropa { get; set; }

        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }

        public string UnidadFaenaId { get; set; }              // RES / MEDIA RES; define nro de piezas (FK a UnidadFaena.Codigo)
        public virtual UnidadFaena UnidadFaena { get; set; }

        public int NumeroGarron { get; set; }                  // nro fisico de gancho; unico por LM
        public long NumeroRomaneo { get; set; }                // correlativo Numerador ROMANEO (Estab+Especie)

        public DateTime Fecha { get; set; }
        public Guid? UsuarioId { get; set; }

        public bool Anulado { get; set; }                      // correccion de errores (devuelve stock)

        public virtual ICollection<RomaneoPieza> Piezas { get; set; }
    }
}
