using MediatR;
using System;

namespace Meat.Application.NumeradoresTropas.DeleteNumeradorTropa
{
    public class DeleteNumeradorTropaRequest : IRequest<DeleteNumeradorTropaResponse>
    {
        public Guid Id { get; set; }
    }
}
