using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Romaneos.GetRenglonesEjecucion
{
    /// <summary>
    /// Renglones de la LM con Cantidad/Faenada/pendiente, el renglon sugerido (menor secuencia
    /// con pendiente) y el proximo garron sugerido. Alimenta el modo hibrido del Tipificador.
    /// </summary>
    public class GetRenglonesEjecucionHandler : IRequestHandler<GetRenglonesEjecucionRequest, GetRenglonesEjecucionResponse>
    {
        private readonly MeatContext context;

        public GetRenglonesEjecucionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetRenglonesEjecucionResponse> Handle(GetRenglonesEjecucionRequest request, CancellationToken cancellationToken)
        {
            var lm = await this.context.ListasMatanzas
                .Include(x => x.Establecimiento).ThenInclude(e => e.Empresa)
                .Include(x => x.Especie)
                .FirstOrDefaultAsync(x => x.Id == request.ListaMatanzaId
                    && x.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);
            if (lm == null)
                throw new ValidationException("La lista de matanza no existe.");

            var renglones = await (
                from d in this.context.ListasMatanzasDetalles
                join t in this.context.Tropas on d.TropaId equals t.Id
                join a in this.context.Almacenes on d.AlmacenId equals a.Id
                join te in this.context.TiposEspecies on d.TipoEspecieId equals te.Id
                where d.ListaMatanzaId == lm.Id
                orderby d.Secuencia
                select new RenglonEjecucionItem
                {
                    RenglonId = d.Id,
                    TropaId = t.Id,
                    NumeroTropa = t.NumeroTropa,
                    AlmacenId = a.Id,
                    AlmacenNombre = a.Nombre,
                    TipoEspecieId = te.Id,
                    TipoEspecieNombre = te.Nombre,
                    Secuencia = d.Secuencia,
                    Cantidad = d.Cantidad,
                    CantidadFaenada = d.CantidadFaenada,
                    Pendiente = d.Cantidad - d.CantidadFaenada
                })
                .ToListAsync(cancellationToken);

            var ultimoGarron = await this.context.Romaneos
                .Where(r => r.ListaMatanzaId == lm.Id && !r.Anulado)
                .Select(r => (int?)r.NumeroGarron)
                .MaxAsync(cancellationToken) ?? 0;

            var sugerido = renglones.FirstOrDefault(r => r.Pendiente > 0);

            return new GetRenglonesEjecucionResponse
            {
                ListaMatanzaId = lm.Id,
                NumeroLista = lm.NumeroLista,
                EspecieId = lm.EspecieId,
                EspecieNombre = lm.Especie != null ? lm.Especie.Nombre : lm.EspecieId,
                EstadoListaMatanzaId = lm.EstadoListaMatanzaId,
                ProximoGarron = ultimoGarron + 1,
                RenglonSugeridoId = sugerido?.RenglonId,
                Renglones = renglones
            };
        }
    }
}
