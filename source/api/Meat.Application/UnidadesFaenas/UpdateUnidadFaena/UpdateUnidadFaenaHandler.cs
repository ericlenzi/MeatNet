using MediatR;
using Meat.Application.Shared;
using Meat.Application.UnidadesFaenas.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.UnidadesFaenas.UpdateUnidadFaena
{
    public class UpdateUnidadFaenaHandler : IRequestHandler<UpdateUnidadFaenaRequest, UpdateUnidadFaenaResponse>
    {
        private readonly MeatContext context;

        public UpdateUnidadFaenaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<UpdateUnidadFaenaResponse> Handle(UpdateUnidadFaenaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.UnidadesFaenas
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
            if (entity == null)
                throw new ValidationException("La unidad de faena no existe.");

            await UnidadFaenaValidacion.ValidarAsync(
                this.context, request.EspecieId, request.Nombre, request.CantidadCuartos,
                request.PiezasPorAnimal, request.TipoMaterialId, cancellationToken);

            // Una sola unidad por defecto por especie: destildar las demas si esta se marca.
            if (request.PorDefecto)
            {
                var otras = await this.context.UnidadesFaenas
                    .Where(u => u.Id != request.Id && u.EspecieId == request.EspecieId && u.PorDefecto)
                    .ToListAsync(cancellationToken);
                foreach (var o in otras) o.PorDefecto = false;
            }

            entity.EspecieId = request.EspecieId;
            entity.Nombre = request.Nombre;
            entity.CantidadCuartos = request.CantidadCuartos;
            entity.PiezasPorAnimal = request.PiezasPorAnimal;
            entity.PorDefecto = request.PorDefecto;
            entity.TipoMaterialId = request.TipoMaterialId;
            entity.ERP_Codigo = request.ERP_Codigo;
            entity.Activo = request.Activo;
            entity.FechaActualizacion = DateTime.Now;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateUnidadFaenaResponse();
        }
    }
}
