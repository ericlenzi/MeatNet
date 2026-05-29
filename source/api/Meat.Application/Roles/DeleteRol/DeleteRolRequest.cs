using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Roles.DeleteRol
{
    public class DeleteRolRequest : IRequest<DeleteRolResponse>
    {
        [Required]
        public string Codigo { get; set; }
    }
}
