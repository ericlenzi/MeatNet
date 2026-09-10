using System;
using System.Collections.Generic;
using System.Linq;

namespace Meat.Application.EvaluacionFaena.Shared
{
    /// <summary>
    /// Resultado del calculo de la Liberacion: los movimientos de camara que se escribirian y
    /// las piezas que lo impiden. La liberacion es todo o nada, asi que con un solo problema
    /// no se escribe nada (ver LiberarJornadaHandler).
    /// </summary>
    public class LiberacionPlan
    {
        public List<MovimientoPlaneado> Movimientos { get; set; } = new List<MovimientoPlaneado>();
        public List<ProblemaLiberacion> Problemas { get; set; } = new List<ProblemaLiberacion>();

        /// <summary>Piezas no anuladas pendientes de liberar (las que este plan intenta procesar).</summary>
        public int PiezasAProcesar { get; set; }

        /// <summary>Piezas de la jornada que ya estaban liberadas y se saltean (R-L7).</summary>
        public int PiezasYaLiberadas { get; set; }

        /// <summary>
        /// Piezas de reses condenadas enteras (R-E23). Se fijan igual al liberar, porque la
        /// jornada queda cerrada, pero no generan existencia: esa carne no entra a camara.
        /// </summary>
        public List<PiezaDecomisada> Decomisadas { get; set; } = new List<PiezaDecomisada>();

        /// <summary>Kilos condenados de la jornada: la merma sanitaria que no llega a camara.</summary>
        public double KilosDecomisados => this.Decomisadas.Sum(d => d.Peso);

        public bool TieneProblemas => this.Problemas.Count > 0;

        /// <summary>Piezas distintas que llegaron a generar movimientos.</summary>
        public int PiezasLiberables => this.Movimientos.Select(m => m.PiezaId).Distinct().Count();
    }

    /// <summary>Un movimiento de camara todavia no persistido.</summary>
    public class MovimientoPlaneado
    {
        public Guid PiezaId { get; set; }
        public Guid RomaneoId { get; set; }
        public long NumeroRomaneo { get; set; }
        public int NumeroGarron { get; set; }
        public string Letra { get; set; }

        public string TipoMovimientoId { get; set; }
        public Guid AlmacenId { get; set; }
        public string AlmacenNombre { get; set; }
        public Guid MaterialId { get; set; }
        public string MaterialCodigo { get; set; }
        public string MaterialNombre { get; set; }
        public int Cantidad { get; set; }
        public double Peso { get; set; }
        public Guid? TransformacionId { get; set; }

        public Guid? TropaId { get; set; }
        public string EspecieId { get; set; }
        public string TipoEspecieId { get; set; }
        public string Referencia { get; set; }
    }

    /// <summary>Pieza de una res condenada: se libera sin generar movimiento de camara.</summary>
    public class PiezaDecomisada
    {
        public Guid PiezaId { get; set; }
        public Guid RomaneoId { get; set; }
        public long NumeroRomaneo { get; set; }
        public int NumeroGarron { get; set; }
        public string Letra { get; set; }
        public double Peso { get; set; }
        public string MotivoDecomisoNombre { get; set; }
    }

    /// <summary>Una pieza que no se puede liberar, con el motivo en lenguaje del operario.</summary>
    public class ProblemaLiberacion
    {
        public Guid PiezaId { get; set; }
        public long NumeroRomaneo { get; set; }
        public int NumeroGarron { get; set; }
        public string Letra { get; set; }
        public string Motivo { get; set; }

        /// <summary>Identifica la pieza en un mensaje de error: "romaneo 12, garron 34 (A)".</summary>
        public string Ubicacion => string.IsNullOrEmpty(this.Letra)
            ? $"romaneo {this.NumeroRomaneo}, garron {this.NumeroGarron}"
            : $"romaneo {this.NumeroRomaneo}, garron {this.NumeroGarron} ({this.Letra})";
    }

    /// <summary>Proyeccion de la pieza con todo lo que el calculo necesita, en una sola consulta.</summary>
    public class PiezaLiberable
    {
        public Guid PiezaId { get; set; }
        public Guid RomaneoId { get; set; }
        public long NumeroRomaneo { get; set; }
        public int NumeroGarron { get; set; }
        public string Letra { get; set; }
        public double Peso { get; set; }
        public Guid AlmacenDestinoId { get; set; }
        public string AlmacenDestinoNombre { get; set; }
        public Guid? TipificacionId { get; set; }
        public bool TipificacionExiste { get; set; }
        public string TipificacionDescripcion { get; set; }
        public Guid? MaterialId { get; set; }
        public Guid? TropaId { get; set; }
        public string EspecieId { get; set; }
        public string TipoEspecieId { get; set; }
        public bool YaLiberada { get; set; }

        /// <summary>La res fue condenada entera (R-E23): esta pieza no genera existencia.</summary>
        public bool DecomisoTotal { get; set; }

        /// <summary>Esta media res fue condenada entera (R-E27). Tampoco genera existencia.</summary>
        public bool Decomisada { get; set; }

        public string MotivoDecomisoNombre { get; set; }
    }

    /// <summary>Regla de despiece ya resuelta con los nombres de su material destino.</summary>
    public class DespieceAplicable
    {
        public Guid MaterialOrigenId { get; set; }
        public Guid MaterialDestinoId { get; set; }
        public string MaterialDestinoCodigo { get; set; }
        public string MaterialDestinoNombre { get; set; }
        public int Cantidad { get; set; }
        public double Rendimiento { get; set; }
    }
}
