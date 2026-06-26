using MediatR;
using System;

namespace Meat.Application.NumeradoresTropas.GetNumeradorTropa
{
    public class GetNumeradorTropaRequest : IRequest<GetNumeradorTropaResponse>
    {
        public Guid Id { get; set; }
    }
}
