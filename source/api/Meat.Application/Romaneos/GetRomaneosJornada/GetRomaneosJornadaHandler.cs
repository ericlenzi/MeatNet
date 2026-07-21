using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Romaneos.GetRomaneosJornada
{
    /// <summary>Romaneos de la jornada (LM) para la grilla del Tipificador. Incluye anulados.</summary>
    public class GetRomaneosJornadaHandler : IRequestHandler<GetRomaneosJornadaRequest, GetRomaneosJornadaResponse>
    {
        private readonly MeatContext context;

        public GetRomaneosJornadaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetRomaneosJornadaResponse> Handle(GetRomaneosJornadaRequest request, CancellationToken cancellationToken)
        {
            var data = await (
                from r in this.context.Romaneos
                join lm in this.context.ListasMatanzas on r.ListaMatanzaId equals lm.Id
                join est in this.context.Establecimientos on lm.EstablecimientoId equals est.Id
                join emp in this.context.Empresas on est.EmpresaId equals emp.Id
                join t in this.context.Tropas on r.TropaId equals t.Id
                join uf in this.context.UnidadesFaenas on r.UnidadFaenaId equals uf.Codigo
                join d in this.context.ListasMatanzasDetalles on r.ListaMatanzaDetalleId equals d.Id
                join te in this.context.TiposEspecies on d.TipoEspecieId equals te.Id
                where r.ListaMatanzaId == request.ListaMatanzaId
                    && emp.CodigoEmpresa == request.CodigoEmpresa
                orderby r.NumeroRomaneo descending
                select new RomaneoJornadaItem
                {
                    Id = r.Id,
                    NumeroRomaneo = r.NumeroRomaneo,
                    NumeroGarron = r.NumeroGarron,
                    NumeroTropa = t.NumeroTropa,
                    TipoEspecieNombre = te.Nombre,
                    UnidadFaenaNombre = uf.Nombre,
                    Anulado = r.Anulado,
                    Fecha = r.Fecha,
                    PesoTotal = r.Piezas.Sum(p => p.Peso),
                    Piezas = r.Piezas
                        .OrderBy(p => p.Letra)
                        .Select(p => new RomaneoPiezaItem
                        {
                            Letra = p.Letra,
                            Peso = p.Peso,
                            AlmacenDestinoNombre = p.AlmacenDestino != null ? p.AlmacenDestino.Nombre : null,
                            TipificacionId = p.TipificacionId,
                            TipificacionDescripcion = p.Tipificacion != null ? p.Tipificacion.Descripcion : null
                        }).ToList()
                })
                .ToListAsync(cancellationToken);

            return new GetRomaneosJornadaResponse { Data = data };
        }
    }
}
