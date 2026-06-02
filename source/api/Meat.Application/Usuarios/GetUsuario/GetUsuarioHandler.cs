using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Usuarios.GetUsuario
{

    public class GetUsuarioHandler : IRequestHandler<GetUsuarioRequest, GetUsuarioResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public GetUsuarioHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<GetUsuarioResponse> Handle(GetUsuarioRequest request, CancellationToken cancellationToken)
        {
            var usuario = await this.context.Usuarios
                .Include(u => u.Empresa)
                .FirstOrDefaultAsync(p => p.Id == request.Id && p.Empresa.CodigoEmpresa == request.CodigoEmpresa);

            return this.mapper.Map<GetUsuarioResponse>(usuario);
        }
    }
}
