using MediatR;
using Meat.Application.Shared;
using System;

namespace Meat.Application.DespiecesMateriales.GetDespiecesMateriales
{
    public class GetDespiecesMaterialesRequest : RequestListBase, IRequest<GetDespiecesMaterialesResponse>
    {
        public bool? Estado { get; set; }
        public Guid? MaterialOrigenId { get; set; }
    }
}
