using Meat.Domain.TiposEstadosTropas;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.Tropas
{
    /// <summary>
    /// Historial append-only del ciclo de vida de una Tropa. Cada proceso que la
    /// afecta (recepcion, anulacion y, a futuro, planificacion, faena, romaneo)
    /// registra aqui un evento. Es la fuente de verdad de la trazabilidad de la
    /// tropa; nunca se edita ni se borra.
    /// </summary>
    public class TropaMovimiento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid TropaId { get; set; }
        public virtual Tropa Tropa { get; set; }

        public int Secuencia { get; set; }              // orden del evento dentro de la tropa (1..n)
        public DateTime Fecha { get; set; }
        public Guid? UsuarioId { get; set; }

        public string TipoMovimiento { get; set; }      // ver TiposMovimientoTropa

        // Estado de la tropa (entera) luego del evento. FK: siempre debe existir en
        // el catalogo TiposEstadosTropas.
        public string EstadoResultanteId { get; set; }
        public virtual TipoEstadoTropa EstadoResultante { get; set; }

        public string Detalle { get; set; }             // texto legible del evento (snapshot)

        // Referencia opcional al documento que origino el evento
        public string ReferenciaTipo { get; set; }      // ej: "INGRESO", "LISTA_MATANZA"
        public Guid? ReferenciaId { get; set; }
    }
}
