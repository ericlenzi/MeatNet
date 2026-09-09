using System;
using MediatR;

namespace Meat.Application.UnidadesFaenas.DeleteUnidadFaena
{
    public class DeleteUnidadFaenaRequest : IRequest<DeleteUnidadFaenaResponse>
    {
        public Guid Id { get; set; }
    }
}
