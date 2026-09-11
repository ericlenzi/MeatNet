using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Domain.EstablecimientosEspecies;
using Meat.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Establecimientos.UpdateEstablecimiento
{
    public class UpdateEstablecimientoHandler : IRequestHandler<UpdateEstablecimientoRequest, UpdateEstablecimientoResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public UpdateEstablecimientoHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<UpdateEstablecimientoResponse> Handle(UpdateEstablecimientoRequest request, CancellationToken cancellationToken)
        {
            var especies = (request.Especies ?? Enumerable.Empty<EstablecimientoEspecieInput>())
                .Where(e => !string.IsNullOrEmpty(e?.EspecieId))
                .GroupBy(e => e.EspecieId)
                .Select(g => g.First())
                .ToList();

            if (especies.Count == 0)
                throw new ValidationException("El establecimiento debe tener al menos una especie asignada.");

            var entity = await this.context.Establecimientos
                .Include(x => x.Especies)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new ValidationException("El establecimiento no existe");

            this.mapper.Map(request, entity);
            entity.FechaActualizacion = DateTime.Now;

            // Las especies se actualizan por diferencia, no reemplazando la lista entera: la fila
            // lleva la merma de oreo de la planta, y borrarla y recrearla en cada guardado
            // perderia ese valor.
            var actuales = entity.Especies?.ToList() ?? new List<EstablecimientoEspecie>();

            foreach (var quitada in actuales.Where(a => especies.All(e => e.EspecieId != a.EspecieId)))
                this.context.EstablecimientosEspecies.Remove(quitada);

            foreach (var especie in especies)
            {
                var fila = actuales.FirstOrDefault(a => a.EspecieId == especie.EspecieId);

                if (fila == null)
                {
                    this.context.EstablecimientosEspecies.Add(new EstablecimientoEspecie
                    {
                        Id = Guid.NewGuid(),
                        EstablecimientoId = entity.Id,
                        EspecieId = especie.EspecieId,
                        MermaOreo = especie.MermaOreo,
                        FechaActualizacion = DateTime.Now
                    });
                    continue;
                }

                fila.MermaOreo = especie.MermaOreo;
                fila.FechaActualizacion = DateTime.Now;
            }

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateEstablecimientoResponse();
        }
    }
}
