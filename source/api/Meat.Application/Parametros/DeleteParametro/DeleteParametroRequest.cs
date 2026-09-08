using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.Parametros.DeleteParametro
{
    public class DeleteParametroRequest : RequestBase, IRequest<DeleteParametroResponse>
    {
        public string Codigo { get; set; }
    }
}
