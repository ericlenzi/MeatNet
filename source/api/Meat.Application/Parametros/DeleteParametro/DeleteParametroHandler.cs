using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            var parametro = await this.context.Parametros.FirstOrDefaultAsync(x => x.Id == request.Id);

            if (parametro == null)
            {
                throw new ValidationException("El parámetro no existe");
            }

            this.context.Remove(parametro);

            #region ParametrosSucursales

            var parametrosSucursales = context.ParametrosSucursales.Where(x => x.ParametroId == request.Id);
            foreach (var item in parametrosSucursales)
            {
                context.ParametrosSucursales.Remove(item);
            }

            #endregion ParametrosSucursales

            await this.context.SaveChangesAsync();

            return new DeleteParametroResponse();
        }
    }
}
