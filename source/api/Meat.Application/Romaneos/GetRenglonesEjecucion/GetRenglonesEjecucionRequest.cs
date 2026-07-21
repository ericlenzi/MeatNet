using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meat.Application.Romaneos.GetRenglonesEjecucion
{
    public class GetRenglonesEjecucionRequest : IRequest<GetRenglonesEjecucionResponse>
    {
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }

        public Guid ListaMatanzaId { get; set; }
    }

    public class GetRenglonesEjecucionResponse
    {
        public Guid ListaMatanzaId { get; set; }
        public long NumeroLista { get; set; }
        public string EspecieId { get; set; }
        public string EspecieNombre { get; set; }
        public string EstadoListaMatanzaId { get; set; }

        public int ProximoGarron { get; set; }              // ultimo garron de la jornada + 1
        public Guid? RenglonSugeridoId { get; set; }         // menor secuencia con pendiente > 0

        public IEnumerable<RenglonEjecucionItem> Renglones { get; set; } = new List<RenglonEjecucionItem>();
        public IEnumerable<CamaraOption> Camaras { get; set; } = new List<CamaraOption>();  // camaras activas del establecimiento (selector de destino)
    }

    public class CamaraOption
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
    }

    public class RenglonEjecucionItem
    {
        public Guid RenglonId { get; set; }
        public Guid TropaId { get; set; }
        public long NumeroTropa { get; set; }
        public Guid AlmacenId { get; set; }
        public string AlmacenNombre { get; set; }
        public Guid? AlmacenDestinoId { get; set; }         // camara destino planificada en la LM (default del puesto)
        public string AlmacenDestinoNombre { get; set; }
        public string TipoEspecieId { get; set; }
        public string TipoEspecieNombre { get; set; }
        public int Secuencia { get; set; }
        public int Cantidad { get; set; }
        public int CantidadFaenada { get; set; }
        public int Pendiente { get; set; }
    }
}
