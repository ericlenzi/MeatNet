using MediatR;
using Meat.Application.Shared;
using Meat.Application.UnidadesFaenas.Shared;
using Meat.Domain.UnidadesFaenas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
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
            if (string.IsNullOrWhiteSpace(request.Codigo))
                throw new ValidationException("El codigo es requerido.");
            var codigo = request.Codigo.Trim();

            var codigoEnUso = await this.context.UnidadesFaenas
                .AnyAsync(u => u.Codigo == codigo, cancellationToken);
            if (codigoEnUso)
                throw new ValidationException("Ya existe una unidad de faena con ese codigo.");

            await UnidadFaenaValidacion.ValidarAsync(
                this.context, request.EspecieId, request.Nombre, request.CantidadCuartos,
                request.PiezasPorAnimal, request.TipoMaterialId, cancellationToken);

            // Una sola unidad por defecto por especie: destildar las demas si esta se marca.
            if (request.PorDefecto)
            {
                var otras = await this.context.UnidadesFaenas
                    .Where(u => u.EspecieId == request.EspecieId && u.PorDefecto)
                    .ToListAsync(cancellationToken);
                foreach (var o in otras) o.PorDefecto = false;
            }

            var entity = UnidadFaenaFactory.Create();
            entity.Codigo = codigo;
            entity.EspecieId = request.EspecieId;
            entity.Nombre = request.Nombre;
            entity.CantidadCuartos = request.CantidadCuartos;
            entity.PiezasPorAnimal = request.PiezasPorAnimal;
            entity.PorDefecto = request.PorDefecto;
            entity.TipoMaterialId = request.TipoMaterialId;
            entity.ERP_Codigo = request.ERP_Codigo;

            this.context.UnidadesFaenas.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateUnidadFaenaResponse { Id = entity.Id };
        }
    }
}
