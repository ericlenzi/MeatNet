using MediatR;
using Meat.Application.Shared;
using System;

namespace Meat.Application.IngresosHaciendas.GetIngresosHaciendas
{
    public class GetIngresosHaciendasRequest : RequestListBase, IRequest<GetIngresosHaciendasResponse>
    {
        public Guid? EstablecimientoId { get; set; }
        public string EstadoIngresoId { get; set; }
    }
}
