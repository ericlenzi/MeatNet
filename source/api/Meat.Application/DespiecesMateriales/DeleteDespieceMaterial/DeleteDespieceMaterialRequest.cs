using MediatR;
using System;

namespace Meat.Application.DespiecesMateriales.DeleteDespieceMaterial
{
    public class DeleteDespieceMaterialRequest : IRequest<DeleteDespieceMaterialResponse>
    {
        public Guid Id { get; set; }
    }
}
