using MediatR;
using System;

namespace Meat.Application.Parametros.GetParametro
{
    public class GetParametroRequest : IRequest<GetParametroResponse>
    {
        public Guid Id { get; set; }
    }
}
