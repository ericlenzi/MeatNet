using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Numeradores.UpdateNumerador
{
    public class UpdateNumeradorHandler : IRequestHandler<UpdateNumeradorRequest, UpdateNumeradorResponse>
    {
        private readonly MeatContext context;

        public UpdateNumeradorHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<UpdateNumeradorResponse> Handle(UpdateNumeradorRequest request, CancellationToken cancellationToken)
        {
            var entity = await (
                from n in this.context.Numeradores
                join e in this.context.Establecimientos on n.EstablecimientoId equals e.Id
                join emp in this.context.Empresas on e.EmpresaId equals emp.Id
                where n.Id == request.Id && emp.CodigoEmpresa == request.CodigoEmpresa
                select n
            ).FirstOrDefaultAsync(cancellationToken);
            if (entity == null)
                throw new ValidationException("El numerador no existe.");

            entity.Descripcion = request.Descripcion;
            entity.TipoNumerador = request.TipoNumerador;
            entity.UltimoNumero = request.UltimoNumero;
            entity.Activo = request.Activo;
            entity.FechaActualizacion = DateTime.Now;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateNumeradorResponse();
        }
    }
}
