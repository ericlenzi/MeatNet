using MediatR;
using Meat.Application.Shared;
using Meat.Domain.UnidadesFaenas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.UnidadesFaenas.CreateUnidadFaena
{
    public class CreateUnidadFaenaHandler : IRequestHandler<CreateUnidadFaenaRequest, CreateUnidadFaenaResponse>
    {
        private readonly MeatContext context;

        public CreateUnidadFaenaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateUnidadFaenaResponse> Handle(CreateUnidadFaenaRequest request, CancellationToken cancellationToken)
        {
            var especieExiste = await this.context.Especies.AnyAsync(e => e.Codigo == request.EspecieId, cancellationToken);
            if (!especieExiste)
                throw new ValidationException("La especie indicada no existe.");

            var numeroEnUso = await this.context.UnidadesFaenas
                .AnyAsync(u => u.EspecieId == request.EspecieId && u.Numero == request.Numero, cancellationToken);
            if (numeroEnUso)
                throw new ValidationException("Ya existe una unidad de faena con ese numero para la especie.");

            if (request.PiezasPorAnimal < 1)
                throw new ValidationException("Las piezas por animal deben ser al menos 1.");

            // Una sola unidad por defecto por especie: destildar las demas si esta se marca.
            if (request.PorDefecto)
            {
                var otras = await this.context.UnidadesFaenas
                    .Where(u => u.EspecieId == request.EspecieId && u.PorDefecto)
                    .ToListAsync(cancellationToken);
                foreach (var o in otras) o.PorDefecto = false;
            }

            var entity = UnidadFaenaFactory.Create();
            entity.EspecieId = request.EspecieId;
            entity.Numero = request.Numero;
            entity.Nombre = request.Nombre;
            entity.CantidadCuartos = request.CantidadCuartos;
            entity.PiezasPorAnimal = request.PiezasPorAnimal;
            entity.PorDefecto = request.PorDefecto;
            entity.CodigoMaterial = request.CodigoMaterial;
            entity.ERP_Codigo = request.ERP_Codigo;
            entity.Activo = true;

            this.context.UnidadesFaenas.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateUnidadFaenaResponse { Id = entity.Id };
        }
    }
}
