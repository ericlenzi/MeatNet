using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meat.Application.EvaluacionFaena.PrevisualizarLiberacion
{
    public class PrevisualizarLiberacionRequest : IRequest<PrevisualizarLiberacionResponse>
    {
        [JsonIgnore]
        public string EmpresaId { get; set; }

        public Guid ListaMatanzaId { get; set; }
    }

    public class PrevisualizarLiberacionResponse
    {
        public Guid ListaMatanzaId { get; set; }
        public long NumeroLista { get; set; }
        public string EstadoListaMatanzaId { get; set; }

        /// <summary>La jornada esta cerrada, condicion para liberar (O-1).</summary>
        public bool JornadaFinalizada { get; set; }

        /// <summary>No hay problemas y hay algo para liberar: el boton Liberar puede habilitarse.</summary>
        public bool PuedeLiberar { get; set; }

        /// <summary>Por que no se puede liberar, en una linea, cuando PuedeLiberar es false.</summary>
        public string MotivoBloqueo { get; set; }

        public int PiezasAProcesar { get; set; }
        public int PiezasYaLiberadas { get; set; }

        /// <summary>Kilos netos que quedarian en camara (el cuarteo no duplica).</summary>
        public double KilosAIngresar { get; set; }

        /// <summary>Resumen de lo que quedaria en camara, por material y almacen.</summary>
        public List<ResumenExistenciaItem> Resumen { get; set; } = new List<ResumenExistenciaItem>();

        /// <summary>Piezas que impiden liberar, con el motivo.</summary>
        public List<ProblemaItem> Problemas { get; set; } = new List<ProblemaItem>();
    }

    public class ResumenExistenciaItem
    {
        public Guid AlmacenId { get; set; }
        public string AlmacenNombre { get; set; }
        public Guid MaterialId { get; set; }
        public string MaterialCodigo { get; set; }
        public string MaterialNombre { get; set; }
        public int Cantidad { get; set; }
        public double Peso { get; set; }
    }

    public class ProblemaItem
    {
        public Guid PiezaId { get; set; }
        public long NumeroRomaneo { get; set; }
        public int NumeroGarron { get; set; }
        public string Letra { get; set; }
        public string Motivo { get; set; }
    }
}
