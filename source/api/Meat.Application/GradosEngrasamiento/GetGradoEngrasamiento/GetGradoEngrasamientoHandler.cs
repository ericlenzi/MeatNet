using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.GradosEngrasamiento.GetGradoEngrasamiento
{
    public class GetGradoEngrasamientoHandler : IRequestHandler<GetGradoEngrasamientoRequest, GetGradoEngrasamientoResponse>
    {
        private readonly MeatContext context;

        public GetGradoEngrasamientoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetGradoEngrasamientoResponse> Handle(GetGradoEngrasamientoRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.GradosEngrasamiento
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                return null;

            return new GetGradoEngrasamientoResponse
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
