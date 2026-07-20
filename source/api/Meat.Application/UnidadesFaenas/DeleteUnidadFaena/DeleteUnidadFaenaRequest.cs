using MediatR;
using System;

namespace Meat.Application.UnidadesFaenas.DeleteUnidadFaena
{
    public class DeleteUnidadFaenaRequest : IRequest<DeleteUnidadFaenaResponse>
    {
        public Guid Id { get; set; }
    }
}
