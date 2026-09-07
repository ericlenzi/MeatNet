using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.EvaluacionFaena.LiberarJornada
{
    public class LiberarJornadaRequest : IRequest<LiberarJornadaResponse>
    {
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
        [JsonIgnore]
        public Guid UsuarioId { get; set; }

        public Guid ListaMatanzaId { get; set; }
    }

    public class LiberarJornadaResponse
    {
        /// <summary>Movimientos de camara escritos en el log.</summary>
        public int MovimientosGenerados { get; set; }

        /// <summary>Piezas que pasaron a Liberado en esta corrida.</summary>
        public int PiezasLiberadas { get; set; }

        /// <summary>Romaneos que quedaron definitivos en esta corrida.</summary>
        public int RomaneosLiberados { get; set; }

        /// <summary>Piezas que ya estaban liberadas y se saltearon (R-L7).</summary>
        public int PiezasYaLiberadas { get; set; }

        /// <summary>Kilos que ingresaron a camara (suma de las altas, sin las bajas por cuarteo).</summary>
        public double KilosIngresados { get; set; }
    }
}
