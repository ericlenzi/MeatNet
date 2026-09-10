using MediatR;

namespace Meat.Application.Denticiones.GetDenticion
{
    public class GetDenticionRequest : IRequest<GetDenticionResponse>
    {
        public string Codigo { get; set; }
    }
}
