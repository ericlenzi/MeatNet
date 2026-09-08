using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.IngresosHaciendas.DeleteIngresoHacienda
{
    public class DeleteIngresoHaciendaRequest : RequestBase, IRequest<DeleteIngresoHaciendaResponse>
    {
        public Guid Id { get; set; }

    }
}
