using MediatR;

namespace Meat.Application.GradosEngrasamiento.DeleteGradoEngrasamiento
{
    public class DeleteGradoEngrasamientoRequest : IRequest<DeleteGradoEngrasamientoResponse>
    {
        public string Codigo { get; set; }
    }
}
