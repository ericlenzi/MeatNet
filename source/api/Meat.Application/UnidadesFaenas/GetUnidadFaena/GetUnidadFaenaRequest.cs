using MediatR;
using System;

namespace Meat.Application.UnidadesFaenas.GetUnidadFaena
{
    public class GetUnidadFaenaRequest : IRequest<GetUnidadFaenaResponse>
    {
        public Guid Id { get; set; }
    }
}
