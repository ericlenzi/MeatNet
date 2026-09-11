using Meat.Application.Shared;
using System;
using System.Collections.Generic;

namespace Meat.Application.Puestos.GetPuestos
{
    public class GetPuestosResponse : ResponseListBase<IEnumerable<PuestoItem>>
    {
    }

    public class PuestoItem
    {
        public Guid Id { get; set; }
        public string CodigoPuesto { get; set; }
        public string Nombre { get; set; }
        public Guid EstablecimientoId { get; set; }
        public string EstablecimientoNombre { get; set; }
        public string EspecieId { get; set; }
        public string EspecieNombre { get; set; }
        public string TipoPuestoId { get; set; }
        public string TipoPuestoNombre { get; set; }
        public string TipoMedicionId { get; set; }
        public string TipoMedicionNombre { get; set; }
        public bool Activo { get; set; }
    }
}
