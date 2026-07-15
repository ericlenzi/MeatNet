using Meat.Domain.Especies;
using Meat.Domain.Establecimientos;
using Meat.Domain.TiposEstadosListasMatanzas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.ListasMatanzas
{
    /// <summary>
    /// Lista de Matanza (LM): programacion diaria de faena de un Establecimiento
    /// para una Especie. Cabecera del proceso de Planificacion de Faena (Ciclo I paso 2).
    /// </summary>
    public class ListaMatanza
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        // Contexto (aporta el filtro por empresa via Establecimiento.Empresa)
        public Guid EstablecimientoId { get; set; }
        public virtual Establecimiento Establecimiento { get; set; }

        // Especie de la lista (una por LM)
        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }

        public DateTime Fecha { get; set; }                // dia de faena (solo fecha)
        public long NumeroLista { get; set; }              // correlativo por establecimiento

        // Estado
        public string EstadoListaMatanzaId { get; set; }
        public virtual TipoEstadoListaMatanza EstadoListaMatanza { get; set; }

        public int Version { get; set; }                   // contador de versiones post-confirmacion

        public DateTime? FechaConfirmacion { get; set; }
        public Guid? UsuarioConfirmacionId { get; set; }
        public DateTime? FechaInicioEjecucion { get; set; }
        public DateTime? FechaFinalizacion { get; set; }

        public DateTime FechaActualizacion { get; set; }

        public virtual ICollection<ListaMatanzaDetalle> Renglones { get; set; }
        public virtual ICollection<ListaMatanzaMovimiento> Movimientos { get; set; }
    }
}
