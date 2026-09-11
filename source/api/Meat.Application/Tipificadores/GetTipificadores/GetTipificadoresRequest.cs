using MediatR;
using Meat.Application.Shared;
using System;

namespace Meat.Application.Tipificadores.GetTipificadores
{
    public class GetTipificadoresRequest : RequestListBase, IRequest<GetTipificadoresResponse>
    {
        public Guid? EstablecimientoId { get; set; }
        public string EspecieId { get; set; }
        public bool? Estado { get; set; }
    }
}
