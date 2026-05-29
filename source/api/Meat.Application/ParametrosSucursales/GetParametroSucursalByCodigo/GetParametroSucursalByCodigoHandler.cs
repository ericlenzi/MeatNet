using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ParametrosSucursales.GetParametroSucursalByCodigo
{
    public class GetParametroSucursalByCodigoHandler : IRequestHandler<GetParametroSucursalByCodigoRequest, GetParametroSucursalByCodigoResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public GetParametroSucursalByCodigoHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<GetParametroSucursalByCodigoResponse> Handle(GetParametroSucursalByCodigoRequest request, CancellationToken cancellationToken)
        {
            var parametro = await this.context.ParametrosSucursales.Include(x => x.Parametro).FirstOrDefaultAsync(x => x.Parametro.Codigo == request.Codigo);

            if (parametro == null)
            {
                return new GetParametroSucursalByCodigoResponse()
                {
                    Valor = "99999"
                };
            }

            return this.mapper.Map<GetParametroSucursalByCodigoResponse>(parametro);
        }
    }
}
