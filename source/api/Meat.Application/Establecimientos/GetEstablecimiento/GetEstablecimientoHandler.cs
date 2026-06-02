using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Establecimientos.GetEstablecimiento
{
    public class GetEstablecimientoHandler : IRequestHandler<GetEstablecimientoRequest, GetEstablecimientoResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public GetEstablecimientoHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<GetEstablecimientoResponse> Handle(GetEstablecimientoRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Establecimientos
                .Include(x => x.Sucursal).ThenInclude(s => s.Empresa)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.Sucursal.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            return this.mapper.Map<GetEstablecimientoResponse>(entity);
        }
    }
}
