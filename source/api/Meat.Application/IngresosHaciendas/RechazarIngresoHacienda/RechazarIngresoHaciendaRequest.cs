using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.IngresosHaciendas.RechazarIngresoHacienda
{
    public class RechazarIngresoHaciendaRequest : RequestBase, IRequest<RechazarIngresoHaciendaResponse>
    {
        public Guid Id { get; set; }

    }

    public class RechazarIngresoHaciendaResponse
    {
    }
}
