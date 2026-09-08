using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.Tropas.GetTropasDisponibles
{
    public class GetTropasDisponiblesRequest : RequestBase, IRequest<GetTropasDisponiblesResponse>
    {

        public Guid? EstablecimientoId { get; set; }
    }
}
