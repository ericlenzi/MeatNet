using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.DespiecesMateriales.UpdateDespieceMaterial
{
    public class UpdateDespieceMaterialRequest : IRequest<UpdateDespieceMaterialResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public Guid MaterialOrigenId { get; set; }
        public Guid MaterialDestinoId { get; set; }
        public int Cantidad { get; set; }
        public double Rendimiento { get; set; }
        public bool Activo { get; set; }
    }
}
