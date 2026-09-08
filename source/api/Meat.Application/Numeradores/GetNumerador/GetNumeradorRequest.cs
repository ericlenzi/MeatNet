using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.Numeradores.GetNumerador
{
    public class GetNumeradorRequest : RequestBase, IRequest<GetNumeradorResponse>
    {
        public Guid Id { get; set; }

    }
}
