using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;

namespace Meat.Application.Especies.DeleteEspecie
{
    public class DeleteEspecieHandler : IRequestHandler<DeleteEspecieRequest, DeleteEspecieResponse>
    {
        private readonly MeatContext context;

        public DeleteEspecieHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteEspecieResponse> Handle(DeleteEspecieRequest request, CancellationToken cancellationToken)
        {
            var especie = await this.context.Especies
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo);

            if (especie == null)
                throw new ValidationException("La especie no existe");

            var tieneEstablecimientos = await this.context.EstablecimientosEspecies
                .AnyAsync(ee => ee.EspecieId == request.Codigo, cancellationToken);
            if (tieneEstablecimientos)
                throw new ValidationException("No se puede eliminar la especie porque tiene establecimientos asignados.");

            this.context.Remove(especie);

            await this.context.SaveChangesAsync();

            return new DeleteEspecieResponse();
        }
    }
}
