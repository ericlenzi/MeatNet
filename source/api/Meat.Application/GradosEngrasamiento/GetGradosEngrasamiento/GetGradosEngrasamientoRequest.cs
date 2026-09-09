using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.GradosEngrasamiento.GetGradosEngrasamiento
{
    public class GetGradosEngrasamientoRequest : RequestListBase, IRequest<GetGradosEngrasamientoResponse>
    {
        public bool? Estado { get; set; }
        public string EspecieId { get; set; }
    }
}
