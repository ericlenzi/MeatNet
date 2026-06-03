using System;
using System.Collections.Generic;

namespace Meat.Application.Establecimientos.GetEstablecimientos
{
    public class GetEstablecimientosItem
    {
        public Guid Id { get; set; }
        public string CodigoEstablecimiento { get; set; }
        public string Nombre { get; set; }
        public Guid SucursalId { get; set; }
        public string SucursalNombre { get; set; }
        public IEnumerable<EspecieItem> Especies { get; set; }
        public string NumeroSenasa { get; set; }
        public string NumeroOncca { get; set; }
        public Guid EmpresaId { get; set; }
        public string EmpresaNombre { get; set; }
        public bool Activo { get; set; }
    }
}