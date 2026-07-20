using System;
using System.Collections.Generic;

namespace Meat.Application.Almacenes.GetAlmacenes
{
    public class GetAlmacenesResponse
    {
        public IEnumerable<AlmacenItem> Data { get; set; } = new List<AlmacenItem>();
    }

    public class AlmacenItem
    {
        public Guid Id { get; set; }
        public string CodigoAlmacen { get; set; }
        public string Nombre { get; set; }
        public int Capacidad { get; set; }
        public string TipoAlmacenId { get; set; }
        public string TipoAlmacenFamilia { get; set; }
        public Guid EstablecimientoId { get; set; }
        public bool Activo { get; set; }
    }
}
