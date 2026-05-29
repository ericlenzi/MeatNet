using AutoMapper;
using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Roles.GetRol
{
    public class GetRolHandler : IRequestHandler<GetRolRequest, GetRolResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public GetRolHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<GetRolResponse> Handle(GetRolRequest request, CancellationToken cancellationToken)
        {
            var rol = await this.context.Roles.FirstOrDefaultAsync(r => r.Codigo == request.Codigo, cancellationToken);

            return this.mapper.Map<GetRolResponse>(rol);
        }
    }
}
