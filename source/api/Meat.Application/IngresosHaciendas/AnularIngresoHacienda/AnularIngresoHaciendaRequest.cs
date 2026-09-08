using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.IngresosHaciendas.AnularIngresoHacienda
{
    public class AnularIngresoHaciendaRequest : RequestBase, IRequest<AnularIngresoHaciendaResponse>
    {
        public Guid Id { get; set; }


    }

    public class AnularIngresoHaciendaResponse
    {
    }
}
