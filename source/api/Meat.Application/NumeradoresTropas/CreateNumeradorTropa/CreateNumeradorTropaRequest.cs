using MediatR;
using System;

namespace Meat.Application.NumeradoresTropas.CreateNumeradorTropa
{
    public class CreateNumeradorTropaRequest : IRequest<CreateNumeradorTropaResponse>
    {
        public Guid ClienteEstablecimientoId { get; set; }
        public string EspecieCodigo { get; set; }
        public long UltimoNumeroTropa { get; set; }
    }
}
