using MediatR;

namespace Meat.Application.Especies.GetEspecie
{
    public class GetEspecieRequest : IRequest<GetEspecieResponse>
    {
        public string Codigo { get; set; }
    }
}
