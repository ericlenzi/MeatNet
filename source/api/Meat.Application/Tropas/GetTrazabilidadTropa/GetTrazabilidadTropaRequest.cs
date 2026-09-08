using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.Tropas.GetTrazabilidadTropa
{
    public class GetTrazabilidadTropaRequest : RequestBase, IRequest<GetTrazabilidadTropaResponse>
    {

        public long NumeroTropa { get; set; }

        // Opcional: acota la busqueda al establecimiento activo
        public Guid? EstablecimientoId { get; set; }
    }
}
