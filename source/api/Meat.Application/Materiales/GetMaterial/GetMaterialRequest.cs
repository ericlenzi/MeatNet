using MediatR;
using System;

namespace Meat.Application.Materiales.GetMaterial
{
    public class GetMaterialRequest : IRequest<GetMaterialResponse>
    {
        public Guid Id { get; set; }
    }
}
