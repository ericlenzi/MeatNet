using MediatR;
using System;

namespace Meat.Application.DespiecesMateriales.CreateDespieceMaterial
{
    public class CreateDespieceMaterialRequest : IRequest<CreateDespieceMaterialResponse>
    {
        public Guid MaterialOrigenId { get; set; }
        public Guid MaterialDestinoId { get; set; }
        public int Cantidad { get; set; }
        public double Rendimiento { get; set; }
    }
}
