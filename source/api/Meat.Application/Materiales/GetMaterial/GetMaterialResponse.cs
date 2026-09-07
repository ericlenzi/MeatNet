using System;

namespace Meat.Application.Materiales.GetMaterial
{
    public class GetMaterialResponse
    {
        public Guid Id { get; set; }
        public string CodigoMaterial { get; set; }
        public string Nombre { get; set; }
        public string TipoMaterialId { get; set; }
        public string TipoMaterialNombre { get; set; }
        public string UnidadMedidaId { get; set; }
        public string UnidadMedidaNombre { get; set; }
        public string PesoTeorico { get; set; }
        public string ERP_Codigo { get; set; }
        public bool Activo { get; set; }
    }
}
