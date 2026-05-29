using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Roles.GetRoles
{
    public class GetRolesHandler : IRequestHandler<GetRolesRequest, GetRolesResponse>
    {
        private readonly MeatContext context;

        public GetRolesHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetRolesResponse> Handle(GetRolesRequest request, CancellationToken cancellationToken)
        {
            var roles = await this.context.Roles
                .OrderBy(r => r.Nombre)
                .ToListAsync(cancellationToken);

            return new GetRolesResponse { Data = roles };
        }
    }
}
