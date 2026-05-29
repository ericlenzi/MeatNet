using MediatR;
using System;

namespace Meat.Application.Parametros.DeleteParametro
{
    public class DeleteParametroRequest : IRequest<DeleteParametroResponse>
    {
        public Guid Id { get; set; }
    }
}
