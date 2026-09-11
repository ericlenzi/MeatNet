using Meat.Application.Shared;
using MediatR;
using System;
using System.Collections.Generic;

namespace Meat.Application.Romaneos.GetRenglonesEjecucion
{
    public class GetRenglonesEjecucionRequest : RequestBase, IRequest<GetRenglonesEjecucionResponse>
    {

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

        // Cabecera del puesto: donde se faena esta jornada, quien tipifica y con que se mide.
        public Guid? PuestoId { get; set; }
        public string PuestoCodigo { get; set; }
        public string PuestoNombre { get; set; }
        public Guid? TipificadorSugeridoId { get; set; }     // el marcado PorDefecto del establecimiento + especie
        public string TipoMedicionSugeridoId { get; set; }   // el configurado en el puesto

        public IEnumerable<RenglonEjecucionItem> Renglones { get; set; } = new List<RenglonEjecucionItem>();
        public IEnumerable<CamaraOption> Camaras { get; set; } = new List<CamaraOption>();  // camaras activas del establecimiento (selector de destino)
        public IEnumerable<TipificadorOption> Tipificadores { get; set; } = new List<TipificadorOption>();  // habilitados para el establecimiento + especie
        public IEnumerable<TipoMedicionOption> TiposMediciones { get; set; } = new List<TipoMedicionOption>();  // metodos de medicion activos
    }

    public class TipificadorOption
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Matricula { get; set; }
        public bool PorDefecto { get; set; }
    }

    public class TipoMedicionOption
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
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
