using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.ListasMatanzas
{
    /// <summary>
    /// Historial append-only de la Lista de Matanza. Registra cada cambio a partir
    /// de la confirmacion (no reemplaza datos). Es la fuente de verdad de la
    /// trazabilidad; nunca se edita ni se borra.
    /// </summary>
    public class ListaMatanzaMovimiento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid ListaMatanzaId { get; set; }
        public virtual ListaMatanza ListaMatanza { get; set; }

        public int Version { get; set; }                   // version resultante tras el cambio
        public DateTime Fecha { get; set; }
        public Guid UsuarioId { get; set; }

        public string TipoMovimiento { get; set; }         // ver TiposMovimientoLM

        public Guid? TropaId { get; set; }
        public Guid? AlmacenId { get; set; }
        public int? CantidadAnterior { get; set; }
        public int? CantidadNueva { get; set; }
        public int? SecuenciaAnterior { get; set; }
        public int? SecuenciaNueva { get; set; }
        public string Motivo { get; set; }
    }
}
