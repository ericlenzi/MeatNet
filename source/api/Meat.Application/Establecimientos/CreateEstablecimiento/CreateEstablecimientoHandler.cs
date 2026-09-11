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

namespace Meat.Application.Establecimientos.CreateEstablecimiento
{
    public class CreateEstablecimientoHandler : IRequestHandler<CreateEstablecimientoRequest, CreateEstablecimientoResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public CreateEstablecimientoHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<CreateEstablecimientoResponse> Handle(CreateEstablecimientoRequest request, CancellationToken cancellationToken)
        {
            var especies = (request.Especies ?? Enumerable.Empty<EstablecimientoEspecieInput>())
                .Where(e => !string.IsNullOrEmpty(e?.EspecieId))
                .GroupBy(e => e.EspecieId)
                .Select(g => g.First())
                .ToList();

            if (especies.Count == 0)
                throw new ValidationException("El establecimiento debe tener al menos una especie asignada.");

            var existe = await this.context.Establecimientos
                .AnyAsync(e => e.CodigoEstablecimiento == request.CodigoEstablecimiento, cancellationToken);
            if (existe)
                throw new ValidationException("Ya existe un establecimiento con ese codigo.");

            var empresa = await this.context.Empresas
                .FirstOrDefaultAsync(e => e.Id == request.EmpresaId, cancellationToken);
            if (empresa == null)
                throw new ValidationException("La empresa activa no es valida.");

            var entity = new Domain.Establecimientos.Establecimiento
            {
                Id = Guid.NewGuid(),
                Activo = true,
                FechaActualizacion = DateTime.Now,
                EmpresaId = empresa.Id
            };
            this.mapper.Map(request, entity);

            this.context.Establecimientos.Add(entity);

            foreach (var especie in especies)
            {
                this.context.EstablecimientosEspecies.Add(new EstablecimientoEspecie
                {
                    Id = Guid.NewGuid(),
                    EstablecimientoId = entity.Id,
                    EspecieId = especie.EspecieId,
                    MermaOreo = especie.MermaOreo,
                    FechaActualizacion = DateTime.Now
                });
            }

            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateEstablecimientoResponse { Id = entity.Id };
        }
    }
}
