using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Roles.GetRol
{
    public class GetRolRequest : IRequest<GetRolResponse>
    {
        [Required]
        public string Codigo { get; set; }
    }
}
