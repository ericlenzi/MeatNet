using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;

namespace Meat.Application.Parametros.DeleteParametro
{
    public class DeleteParametroHandler : IRequestHandler<DeleteParametroRequest, DeleteParametroResponse>
    {
        private readonly MeatContext context;

        public DeleteParametroHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteParametroResponse> Handle(DeleteParametroRequest request, CancellationToken cancellationToken)
        {
            var parametro = await this.context.Parametros
                .Include(x => x.Empresa)
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo && x.Empresa.CodigoEmpresa == request.CodigoEmpresa);

            if (parametro == null)
            {
                throw new ValidationException("El parametro no existe");
            }

            this.context.Remove(parametro);

            await this.context.SaveChangesAsync();

            return new DeleteParametroResponse();
        }
    }
}
