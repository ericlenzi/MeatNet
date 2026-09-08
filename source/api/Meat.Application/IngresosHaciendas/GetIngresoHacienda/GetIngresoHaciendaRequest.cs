using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.IngresosHaciendas.GetIngresoHacienda
{
    public class GetIngresoHaciendaRequest : RequestBase, IRequest<GetIngresoHaciendaResponse>
    {
        public Guid Id { get; set; }

    }
}
