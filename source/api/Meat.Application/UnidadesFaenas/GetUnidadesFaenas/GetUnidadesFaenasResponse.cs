using Meat.Application.Shared;
using System;
using System.Collections.Generic;

namespace Meat.Application.UnidadesFaenas.GetUnidadesFaenas
{
    public class GetUnidadesFaenasResponse : ResponseListBase<IEnumerable<UnidadFaenaItem>>
    {
    }

    public class UnidadFaenaItem
    {
        public Guid Id { get; set; }
        public string EspecieId { get; set; }
        public string EspecieNombre { get; set; }
        public int Numero { get; set; }
        public string Nombre { get; set; }
        public int CantidadCuartos { get; set; }
        public int PiezasPorAnimal { get; set; }
        public bool PorDefecto { get; set; }
        public string CodigoMaterial { get; set; }
        public string ERP_Codigo { get; set; }
        public bool Activo { get; set; }
    }
}
