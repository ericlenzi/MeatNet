using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.IngresosHaciendas.AnularIngresoHacienda
{
    public class AnularIngresoHaciendaHandler : IRequestHandler<AnularIngresoHaciendaRequest, AnularIngresoHaciendaResponse>
    {
        private readonly MeatContext context;

        public AnularIngresoHaciendaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<AnularIngresoHaciendaResponse> Handle(AnularIngresoHaciendaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.IngresosHaciendas
                .Include(i => i.Establecimiento).ThenInclude(e => e.Empresa)
                .Include(i => i.Tropas)
                .FirstOrDefaultAsync(i => i.Id == request.Id
                    && i.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("El ingreso de hacienda no existe.");

            if (entity.EstadoIngresoId != EstadosIngreso.Aprobado)
                throw new ValidationException("Solo se puede anular un ingreso en estado Aprobado.");

            // Las tropas pasan a Anulada (el numero permanece y no se reutiliza).
            foreach (var tropa in entity.Tropas)
            {
                tropa.EstadoTropaId = EstadosTropa.Anulada;
            }

            entity.EstadoIngresoId = EstadosIngreso.Anulado;
            entity.FechaActualizacion = DateTime.Now;

            await this.context.SaveChangesAsync(cancellationToken);

            return new AnularIngresoHaciendaResponse();
        }
    }
}
