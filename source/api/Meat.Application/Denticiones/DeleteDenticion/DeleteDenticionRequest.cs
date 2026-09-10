using MediatR;

namespace Meat.Application.Denticiones.DeleteDenticion
{
    public class DeleteDenticionRequest : IRequest<DeleteDenticionResponse>
    {
        public string Codigo { get; set; }
    }
}
