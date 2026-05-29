using Meat.Domain.Roles;
using System.Collections.Generic;

namespace Meat.Application.Roles.GetRoles
{
    public class GetRolesResponse
    {
        public IEnumerable<Rol> Data { get; set; }
    }
}
