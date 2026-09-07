using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Materiales.UpdateMaterial
{
    public class UpdateMaterialRequest : IRequest<UpdateMaterialResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string Nombre { get; set; }
        public string TipoMaterialId { get; set; }
        public string UnidadMedidaId { get; set; }
        public string PesoTeorico { get; set; }
        public string ERP_Codigo { get; set; }
        public bool Activo { get; set; }
    }
}
