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
            if (request.EspecieIds == null || !request.EspecieIds.Any(e => !string.IsNullOrEmpty(e)))
                throw new ValidationException("El establecimiento debe tener al menos una especie asignada.");

            var entity = await this.context.Establecimientos
                .Include(x => x.Empresa)
                .Include(x => x.Especies)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("El establecimiento no existe");

            this.mapper.Map(request, entity);
            entity.FechaActualizacion = DateTime.Now;

            // Reemplazar especies: eliminar las existentes y agregar las nuevas
            if (entity.Especies != null && entity.Especies.Any())
                this.context.EstablecimientosEspecies.RemoveRange(entity.Especies);

            if (request.EspecieIds != null)
            {
                foreach (var especieId in request.EspecieIds.Where(e => !string.IsNullOrEmpty(e)).Distinct())
                {
                    this.context.EstablecimientosEspecies.Add(new EstablecimientoEspecie
                    {
                        Id = Guid.NewGuid(),
                        EstablecimientoId = entity.Id,
                        EspecieId = especieId,
                        FechaActualizacion = DateTime.Now
                    });
                }
            }

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateEstablecimientoResponse();
        }
    }
}
