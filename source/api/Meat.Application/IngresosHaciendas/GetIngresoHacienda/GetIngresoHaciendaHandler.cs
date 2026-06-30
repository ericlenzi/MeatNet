using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.IngresosHaciendas.GetIngresoHacienda
{
    public class GetIngresoHaciendaHandler : IRequestHandler<GetIngresoHaciendaRequest, GetIngresoHaciendaResponse>
    {
        private readonly MeatContext context;

        public GetIngresoHaciendaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetIngresoHaciendaResponse> Handle(GetIngresoHaciendaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.IngresosHaciendas
                .Include(i => i.Establecimiento).ThenInclude(e => e.Empresa)
                .Include(i => i.Cliente)
                .Include(i => i.ClienteEstablecimiento)
                .Include(i => i.Provincia)
                .Include(i => i.OrigenHacienda)
                .Include(i => i.UsoHacienda)
                .Include(i => i.EstadoIngreso)
                .Include(i => i.Pesadas).ThenInclude(p => p.TipoEspecie)
                .Include(i => i.Ubicaciones).ThenInclude(u => u.TipoEspecie)
                .Include(i => i.Ubicaciones).ThenInclude(u => u.Almacen)
                .Include(i => i.Ubicaciones).ThenInclude(u => u.EstadoHacienda)
                .Include(i => i.Tropas)
                .FirstOrDefaultAsync(i => i.Id == request.Id
                    && i.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                return null;

            return new GetIngresoHaciendaResponse
            {
                Id = entity.Id,
                NumeroIngreso = entity.NumeroIngreso,
                EstablecimientoId = entity.EstablecimientoId,
                EstablecimientoNombre = entity.Establecimiento?.Nombre,
                FechaHoraIngreso = entity.FechaHoraIngreso,
                NumeroDte = entity.NumeroDte,
                FechaEmisionDte = entity.FechaEmisionDte,
                ClienteId = entity.ClienteId,
                ClienteNombre = entity.Cliente?.Nombre,
                ClienteEstablecimientoId = entity.ClienteEstablecimientoId,
                CodigoRenspa = entity.ClienteEstablecimiento?.CodigoRenspa,
                NumeroCUIG = entity.ClienteEstablecimiento?.NumeroCUIG,
                ProvinciaId = entity.ProvinciaId,
                ProvinciaNombre = entity.Provincia?.Nombre,
                Localidad = entity.Localidad,
                OrigenHaciendaId = entity.OrigenHaciendaId,
                OrigenHaciendaNombre = entity.OrigenHacienda?.Nombre,
                UsoHaciendaId = entity.UsoHaciendaId,
                UsoHaciendaNombre = entity.UsoHacienda?.Nombre,
                Transportista = entity.Transportista,
                Chofer = entity.Chofer,
                PatenteCamion = entity.PatenteCamion,
                PatenteJaula = entity.PatenteJaula,
                PesoBruto = entity.PesoBruto,
                Tara = entity.Tara,
                PesoNeto = entity.PesoNeto,
                EstadoIngresoId = entity.EstadoIngresoId,
                EstadoIngresoNombre = entity.EstadoIngreso?.Nombre,
                FechaAprobacion = entity.FechaAprobacion,
                Pesadas = entity.Pesadas.Select(p => new PesadaItem
                {
                    Id = p.Id,
                    TipoEspecieId = p.TipoEspecieId,
                    TipoEspecieNombre = p.TipoEspecie?.Nombre,
                    PesoIngreso = p.PesoIngreso,
                    UnidadMedida = p.UnidadMedida
                }).ToList(),
                Ubicaciones = entity.Ubicaciones.Select(u => new UbicacionItem
                {
                    Id = u.Id,
                    TropaId = u.TropaId,
                    TipoEspecieId = u.TipoEspecieId,
                    TipoEspecieNombre = u.TipoEspecie?.Nombre,
                    AlmacenId = u.AlmacenId,
                    AlmacenNombre = u.Almacen?.Nombre,
                    Cantidad = u.Cantidad,
                    PesoPromedio = u.PesoPromedio,
                    EstadoHaciendaId = u.EstadoHaciendaId,
                    EstadoHaciendaNombre = u.EstadoHacienda?.Nombre
                }).ToList(),
                Tropas = entity.Tropas.Select(t => new TropaItem
                {
                    Id = t.Id,
                    NumeroTropa = t.NumeroTropa,
                    EspecieCodigo = t.EspecieCodigo,
                    EstadoTropaId = t.EstadoTropaId
                }).ToList()
            };
        }
    }
}
