using System;
using System.Collections.Generic;
using System.Text;

namespace Meat.Queries.SincronizacionTablasOffline.Dto
{
    public class GiftcardDto
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public bool EsAlta { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
