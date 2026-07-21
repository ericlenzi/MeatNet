using MediatR;
using Meat.Application.Shared;
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

            var especieExiste = await this.context.Especies.AnyAsync(e => e.Codigo == request.EspecieId, cancellationToken);
            if (!especieExiste)
                throw new ValidationException("La especie indicada no existe.");

            var codigoEnUso = await this.context.UnidadesFaenas
                .AnyAsync(u => u.Codigo == codigo, cancellationToken);
            if (codigoEnUso)
                throw new ValidationException("Ya existe una unidad de faena con ese codigo.");

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

            var entity = new UnidadFaena
            {
                Codigo = codigo,
                EspecieId = request.EspecieId,
                Nombre = request.Nombre,
                CantidadCuartos = request.CantidadCuartos,
                PiezasPorAnimal = request.PiezasPorAnimal,
                PorDefecto = request.PorDefecto,
                CodigoMaterial = request.CodigoMaterial,
                ERP_Codigo = request.ERP_Codigo,
                Activo = true,
                FechaActualizacion = DateTime.Now
            };

            this.context.UnidadesFaenas.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateUnidadFaenaResponse { Codigo = entity.Codigo };
        }
    }
}
