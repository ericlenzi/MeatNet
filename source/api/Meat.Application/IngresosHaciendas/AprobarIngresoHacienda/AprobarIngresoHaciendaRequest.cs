using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.IngresosHaciendas.AprobarIngresoHacienda
{
    public class AprobarIngresoHaciendaRequest : RequestBase, IRequest<AprobarIngresoHaciendaResponse>
    {
        public Guid Id { get; set; }


    }
}
