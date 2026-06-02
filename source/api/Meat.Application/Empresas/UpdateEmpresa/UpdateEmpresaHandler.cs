using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Empresas.UpdateEmpresa
{
    public class UpdateEmpresaHandler : IRequestHandler<UpdateEmpresaRequest, UpdateEmpresaResponse>
    {
        private const string TipoEmpresaPropia = "PRP";
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public UpdateEmpresaHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<UpdateEmpresaResponse> Handle(UpdateEmpresaRequest request, CancellationToken cancellationToken)
        {
            var empresa = await this.context.Empresas.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (empresa == null)
                throw new ValidationException("La empresa no existe");

            if (request.TipoEmpresaId == TipoEmpresaPropia && empresa.TipoEmpresaId != TipoEmpresaPropia)
            {
                var existePropia = await this.context.Empresas
                    .AnyAsync(e => e.TipoEmpresaId == TipoEmpresaPropia, cancellationToken);

                if (existePropia)
                    throw new ValidationException("Ya existe una empresa propia. Solo puede haber una empresa con tipo PRP.");
            }

            var empresaActiva = await this.context.Empresas
                .FirstOrDefaultAsync(e => e.CodigoEmpresa == request.CodigoEmpresaActiva, cancellationToken);
            if (empresaActiva == null)
                throw new ValidationException("La empresa activa no es valida.");

            this.mapper.Map(request, empresa);
            empresa.EmpresaId = empresaActiva.Id;
            empresa.FechaActualizacion = DateTime.Now;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateEmpresaResponse();
        }
    }
}
