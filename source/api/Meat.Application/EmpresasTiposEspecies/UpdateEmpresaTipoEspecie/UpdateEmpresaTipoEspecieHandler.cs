using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.EmpresasTiposEspecies.UpdateEmpresaTipoEspecie
{
    public class UpdateEmpresaTipoEspecieHandler : IRequestHandler<UpdateEmpresaTipoEspecieRequest, UpdateEmpresaTipoEspecieResponse>
    {
        private readonly MeatContext context;

        public UpdateEmpresaTipoEspecieHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<UpdateEmpresaTipoEspecieResponse> Handle(UpdateEmpresaTipoEspecieRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.EmpresasTiposEspecies
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new ValidationException("La configuracion del tipo de especie no existe.");

            entity.PesoTeorico = request.PesoTeorico;
            entity.ERP_Codigo = request.ERP_Codigo;
            entity.Activo = request.Activo;
            entity.FechaActualizacion = DateTime.Now;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateEmpresaTipoEspecieResponse();
        }
    }
}
