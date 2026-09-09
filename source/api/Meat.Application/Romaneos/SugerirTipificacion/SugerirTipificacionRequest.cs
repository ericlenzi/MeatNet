using Meat.Application.Shared;
using MediatR;
using System;
using System.Collections.Generic;

namespace Meat.Application.Romaneos.SugerirTipificacion
{
    public class SugerirTipificacionRequest : RequestBase, IRequest<SugerirTipificacionResponse>
    {

        public string EspecieId { get; set; }
        public string TipoEspecieId { get; set; }
        public Guid? UnidadFaenaId { get; set; }
        public Guid? DestinoComercialId { get; set; }      // opcional (filtro)
        public double? Peso { get; set; }                    // opcional; si viene, elige por rango
    }

    public class SugerirTipificacionResponse
    {
        public Guid? PropuestaId { get; set; }               // null si nada matchea el peso
        public IEnumerable<TipificacionCandidata> Candidatas { get; set; } = new List<TipificacionCandidata>();
    }

    public class TipificacionCandidata
    {
        // Identidad: es lo que despues viaja como RomaneoPieza.TipificacionId. El Codigo es para
        // mostrar, no para identificar (Tipificacion tiene PK Guid desde la migracion 59).
        public Guid Id { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public Guid? DestinoComercialId { get; set; }
        public string DestinoComercialNombre { get; set; }
        public double PesoDesde { get; set; }
        public double PesoHasta { get; set; }
        public int Puntos { get; set; }
    }
}
