using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Denticiones.GetDenticion
{
    public class GetDenticionHandler : IRequestHandler<GetDenticionRequest, GetDenticionResponse>
    {
        private readonly MeatContext context;

        public GetDenticionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetDenticionResponse> Handle(GetDenticionRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Denticiones
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                return null;

            return new GetDenticionResponse
            {
                Codigo = entity.Codigo,
                Nombre = entity.Nombre,
                EspecieId = entity.EspecieId,
                Orden = entity.Orden,
                Activo = entity.Activo,
            };
        }
    }
}
