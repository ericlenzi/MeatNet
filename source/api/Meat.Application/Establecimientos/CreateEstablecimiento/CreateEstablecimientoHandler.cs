using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System;
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
            var empresa = await this.context.Empresas
                .FirstOrDefaultAsync(e => e.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);
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
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateEstablecimientoResponse { Id = entity.Id };
        }
    }
}
