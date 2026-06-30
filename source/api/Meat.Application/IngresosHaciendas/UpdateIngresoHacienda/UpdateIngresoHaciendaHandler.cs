using MediatR;
using Meat.Application.Shared;
using Meat.Domain.IngresosHaciendas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.IngresosHaciendas.UpdateIngresoHacienda
{
    public class UpdateIngresoHaciendaHandler : IRequestHandler<UpdateIngresoHaciendaRequest, UpdateIngresoHaciendaResponse>
    {
        private readonly MeatContext context;

        public UpdateIngresoHaciendaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<UpdateIngresoHaciendaResponse> Handle(UpdateIngresoHaciendaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.IngresosHaciendas
                .Include(i => i.Establecimiento).ThenInclude(e => e.Empresa)
                .Include(i => i.Pesadas)
                .Include(i => i.Ubicaciones)
                .FirstOrDefaultAsync(i => i.Id == request.Id
                    && i.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("El ingreso de hacienda no existe.");

            if (entity.EstadoIngresoId != EstadosIngreso.Borrador)
                throw new ValidationException("Solo se puede editar un ingreso en estado Borrador.");

            // Cabecera (el establecimiento no se cambia)
            entity.FechaHoraIngreso = request.FechaHoraIngreso;
            entity.NumeroDte = request.NumeroDte;
            entity.FechaEmisionDte = request.FechaEmisionDte;
            entity.ClienteId = request.ClienteId;
            entity.ClienteEstablecimientoId = request.ClienteEstablecimientoId;
            entity.ProvinciaId = request.ProvinciaId;
            entity.Localidad = request.Localidad;
            entity.OrigenHaciendaId = request.OrigenHaciendaId;
            entity.UsoHaciendaId = request.UsoHaciendaId;
            entity.Transportista = request.Transportista;
            entity.Chofer = request.Chofer;
            entity.PatenteCamion = request.PatenteCamion;
            entity.PatenteJaula = request.PatenteJaula;
            entity.PesoBruto = request.PesoBruto;
            entity.Tara = request.Tara;
            entity.PesoNeto = request.PesoBruto - request.Tara;
            entity.FechaActualizacion = System.DateTime.Now;

            // Reemplazar detalle (soft-delete de las lineas previas + alta de las nuevas)
            this.context.IngresosHaciendasPesadas.RemoveRange(entity.Pesadas);
            this.context.IngresosHaciendasUbicaciones.RemoveRange(entity.Ubicaciones);

            var nuevasPesadas = request.Pesadas
                .Select(p => new IngresoHaciendaPesada
                {
                    Id = System.Guid.NewGuid(),
                    IngresoHaciendaId = entity.Id,
                    TipoEspecieId = p.TipoEspecieId,
                    PesoIngreso = p.PesoIngreso,
                    UnidadMedida = "KG"
                })
                .ToList();

            var nuevasUbicaciones = IngresoHaciendaCalculos
                .BuildUbicaciones(entity.Id, request.Pesadas, request.Ubicaciones);

            await this.context.IngresosHaciendasPesadas.AddRangeAsync(nuevasPesadas, cancellationToken);
            await this.context.IngresosHaciendasUbicaciones.AddRangeAsync(nuevasUbicaciones, cancellationToken);

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateIngresoHaciendaResponse();
        }
    }
}
