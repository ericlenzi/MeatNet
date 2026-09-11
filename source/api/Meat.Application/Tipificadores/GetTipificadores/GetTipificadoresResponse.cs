using Meat.Application.Shared;
using System;
using System.Collections.Generic;

namespace Meat.Application.Tipificadores.GetTipificadores
{
    public class GetTipificadoresResponse : ResponseListBase<IEnumerable<TipificadorItem>>
    {
    }

    public class TipificadorItem
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Matricula { get; set; }
        public Guid EstablecimientoId { get; set; }
        public string EstablecimientoNombre { get; set; }
        public string EspecieId { get; set; }
        public string EspecieNombre { get; set; }
        public bool PorDefecto { get; set; }
        public bool Activo { get; set; }
    }
}
