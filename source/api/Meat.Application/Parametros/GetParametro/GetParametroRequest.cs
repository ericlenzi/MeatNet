using System;
using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.Parametros.GetParametro
{
    public class GetParametroRequest : RequestBase, IRequest<GetParametroResponse>
    {
        public Guid Id { get; set; }
    }
}
