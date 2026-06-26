using MediatR;
using System;

namespace Meat.Application.NumeradoresTropas.UpdateNumeradorTropa
{
    public class UpdateNumeradorTropaRequest : IRequest<UpdateNumeradorTropaResponse>
    {
        public Guid Id { get; set; }
        public long UltimoNumeroTropa { get; set; }
    }
}
