using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.Parametros.GetParametro
{
    public class GetParametroRequest : RequestBase, IRequest<GetParametroResponse>
    {
        public string Codigo { get; set; }
    }
}
