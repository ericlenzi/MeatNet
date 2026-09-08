using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.IngresosHaciendas.EnviarAprobacionIngresoHacienda
{
    public class EnviarAprobacionIngresoHaciendaRequest : RequestBase, IRequest<EnviarAprobacionIngresoHaciendaResponse>
    {
        public Guid Id { get; set; }

    }

    public class EnviarAprobacionIngresoHaciendaResponse
    {
    }
}
