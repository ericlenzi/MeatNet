using MediatR;
using System;

namespace Meat.Application.Materiales.DeleteMaterial
{
    public class DeleteMaterialRequest : IRequest<DeleteMaterialResponse>
    {
        public Guid Id { get; set; }
    }
}
