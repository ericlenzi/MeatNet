using System;
using System.Collections.Generic;

namespace Meat.Application.Tropas.GetTrazabilidadTropa
{
    public class GetTrazabilidadTropaResponse
    {
        public IEnumerable<TrazabilidadTropaItem> Data { get; set; } = new List<TrazabilidadTropaItem>();
    }

    /// <summary>Una tropa que coincide con el numero buscado, con su linea de tiempo.</summary>
    public class TrazabilidadTropaItem
    {
        public Guid TropaId { get; set; }
        public long NumeroTropa { get; set; }

        public string EspecieCodigo { get; set; }
        public string EspecieNombre { get; set; }
        public string ClienteNombre { get; set; }
        public string EstablecimientoNombre { get; set; }
        public long NumeroIngreso { get; set; }

        public string EstadoTropaId { get; set; }
        public string EstadoTropaNombre { get; set; }
        public DateTime FechaRecepcion { get; set; }

        public IEnumerable<TrazabilidadMovimientoItem> Movimientos { get; set; } = new List<TrazabilidadMovimientoItem>();
    }

    /// <summary>
    /// Un evento de la linea de tiempo. Proviene del log propio de la tropa
    /// (TropaMovimiento) o del historial de la planificacion (ListaMatanzaMovimiento),
    /// unificados y ordenados por fecha.
    /// </summary>
    public class TrazabilidadMovimientoItem
    {
        public DateTime Fecha { get; set; }
        public string Fase { get; set; }             // "Recepcion" | "Planificacion" | ...
        public string TipoMovimiento { get; set; }   // codigo del evento
        public string EstadoResultanteId { get; set; }
        public string Detalle { get; set; }
        public string Referencia { get; set; }       // ej: "LM #12"
        public string UsuarioNombre { get; set; }
    }
}
