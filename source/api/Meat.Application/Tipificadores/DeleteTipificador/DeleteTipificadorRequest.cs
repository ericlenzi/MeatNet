using MediatR;
using System;

namespace Meat.Application.Tipificadores.DeleteTipificador
{
    public class DeleteTipificadorRequest : IRequest<DeleteTipificadorResponse>
    {
        public Guid Id { get; set; }
    }
}
