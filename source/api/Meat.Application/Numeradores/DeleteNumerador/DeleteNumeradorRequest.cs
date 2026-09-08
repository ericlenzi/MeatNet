using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.Numeradores.DeleteNumerador
{
    public class DeleteNumeradorRequest : RequestBase, IRequest<DeleteNumeradorResponse>
    {
        public Guid Id { get; set; }

    }
}
