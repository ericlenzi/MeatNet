using MediatR;

namespace Meat.Application.Especies.DeleteEspecie
{
    public class DeleteEspecieRequest : IRequest<DeleteEspecieResponse>
    {
        public string Codigo { get; set; }
    }
}
