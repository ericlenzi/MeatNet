using MediatR;

namespace Meat.Application.GradosEngrasamiento.GetGradoEngrasamiento
{
    public class GetGradoEngrasamientoRequest : IRequest<GetGradoEngrasamientoResponse>
    {
        public string Codigo { get; set; }
    }
}
