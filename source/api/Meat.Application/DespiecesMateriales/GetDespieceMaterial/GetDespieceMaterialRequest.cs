using MediatR;
using System;

namespace Meat.Application.DespiecesMateriales.GetDespieceMaterial
{
    public class GetDespieceMaterialRequest : IRequest<GetDespieceMaterialResponse>
    {
        public Guid Id { get; set; }
    }
}
