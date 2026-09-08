using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.Numeradores.GetNumeradores
{
    public class GetNumeradoresRequest : RequestBase, IRequest<GetNumeradoresResponse>
    {

        public Guid? EstablecimientoId { get; set; }
        public bool? Estado { get; set; }
    }
}
