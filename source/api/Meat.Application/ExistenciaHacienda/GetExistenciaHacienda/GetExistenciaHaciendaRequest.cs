using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.ExistenciaHacienda.GetExistenciaHacienda
{
    public class GetExistenciaHaciendaRequest : RequestBase, IRequest<GetExistenciaHaciendaResponse>
    {

        public Guid? EstablecimientoId { get; set; }
    }
}
