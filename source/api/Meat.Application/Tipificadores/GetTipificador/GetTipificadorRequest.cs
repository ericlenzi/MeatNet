using MediatR;
using System;

namespace Meat.Application.Tipificadores.GetTipificador
{
    public class GetTipificadorRequest : IRequest<GetTipificadorResponse>
    {
        public Guid Id { get; set; }
    }
}
