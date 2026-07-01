using MediatR;
using Meat.Application.Shared;
using Meat.Domain.IngresosHaciendas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.IngresosHaciendas.CreateIngresoHacienda
{
    public class CreateIngresoHaciendaHandler : IRequestHandler<CreateIngresoHaciendaRequest, CreateIngresoHaciendaResponse>
    {
        private readonly MeatContext context;

        public CreateIngresoHaciendaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateIngresoHaciendaResponse> Handle(CreateIngresoHaciendaRequest request, CancellationToken cancellationToken)
        {
            // Validar establecimiento activo dentro de la empresa
            var establecimiento = await this.context.Establecimientos
                .Include(e => e.Empresa)
                .FirstOrDefaultAsync(e => e.Id == request.EstablecimientoId
                    && e.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);
            if (establecimiento == null)
                throw new ValidationException("El establecimiento activo no es valido.");

            // La especie debe estar habilitada para el establecimiento
            if (string.IsNullOrEmpty(request.EspecieId))
                throw new ValidationException("Debe indicar la especie del ingreso.");
            var especieHabilitada = await this.context.EstablecimientosEspecies
                .AnyAsync(ee => ee.EstablecimientoId == establecimiento.Id
                    && ee.EspecieId == request.EspecieId, cancellationToken);
            if (!especieHabilitada)
                throw new ValidationException("La especie no esta habilitada para el establecimiento.");

            // Numero de ingreso correlativo por establecimiento (MAX + 1, transaccional)
            var maxNumero = await this.context.IngresosHaciendas
                .Where(i => i.EstablecimientoId == establecimiento.Id)
                .Select(i => (long?)i.NumeroIngreso)
                .MaxAsync(cancellationToken) ?? 0;

            var entity = IngresoHaciendaFactory.Create();
            entity.NumeroIngreso = maxNumero + 1;
            entity.EstablecimientoId = establecimiento.Id;
            entity.EspecieId = request.EspecieId;
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
            entity.PesoNeto = request.Pesadas.Sum(p => p.PesoIngreso);
            entity.EstadoIngresoId = EstadosIngreso.Borrador;

            entity.Pesadas = request.Pesadas
                .Select(p => new IngresoHaciendaPesada
                {
                    Id = Guid.NewGuid(),
                    IngresoHaciendaId = entity.Id,
                    TipoEspecieId = p.TipoEspecieId,
                    PesoIngreso = p.PesoIngreso,
                    UnidadMedida = "KG",
                    IdPesada = p.IdPesada
                })
                .ToList();

            entity.Ubicaciones = IngresoHaciendaCalculos
                .BuildUbicaciones(entity.Id, request.Pesadas, request.Ubicaciones);

            this.context.IngresosHaciendas.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateIngresoHaciendaResponse
            {
                Id = entity.Id,
                NumeroIngreso = entity.NumeroIngreso
            };
        }
    }
}
