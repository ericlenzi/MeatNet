using MediatR;
using System.Text.Json.Serialization;

namespace Meat.Application.Roles.GetRoles
{
    public class GetRolesRequest : IRequest<GetRolesResponse>
    {
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
