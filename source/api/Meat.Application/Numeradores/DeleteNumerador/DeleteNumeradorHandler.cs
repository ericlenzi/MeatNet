using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Numeradores.DeleteNumerador
{
    public class DeleteNumeradorHandler : IRequestHandler<DeleteNumeradorRequest, DeleteNumeradorResponse>
    {
        private readonly MeatContext context;

        public DeleteNumeradorHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteNumeradorResponse> Handle(DeleteNumeradorRequest request, CancellationToken cancellationToken)
        {
            var entity = await (
                from n in this.context.Numeradores
                join e in this.context.Establecimientos on n.EstablecimientoId equals e.Id
                join emp in this.context.Empresas on e.EmpresaId equals emp.Id
                where n.Id == request.Id && emp.CodigoEmpresa == request.CodigoEmpresa
                select n
            ).FirstOrDefaultAsync(cancellationToken);
            if (entity == null)
                throw new ValidationException("El numerador no existe.");

            this.context.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteNumeradorResponse();
        }
    }
}
