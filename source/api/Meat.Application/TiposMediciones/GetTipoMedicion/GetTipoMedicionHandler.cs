using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposMediciones.GetTipoMedicion
{
    public class GetTipoMedicionHandler : IRequestHandler<GetTipoMedicionRequest, GetTipoMedicionResponse>
    {
        private readonly MeatContext context;

        public GetTipoMedicionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetTipoMedicionResponse> Handle(GetTipoMedicionRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.TiposMediciones
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                return null;

            return new GetTipoMedicionResponse
            {
                Codigo = entity.Codigo,
                Nombre = entity.Nombre,
                Activo = entity.Activo,
            };
        }
    }
}
