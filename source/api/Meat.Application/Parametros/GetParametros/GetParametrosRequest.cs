using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.Parametros.GetParametros
{
    public class GetParametrosRequest : RequestListBase, IRequest<GetParametrosResponse>
    {
    }
}
