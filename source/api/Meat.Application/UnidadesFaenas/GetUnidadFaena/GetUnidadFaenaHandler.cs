using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.UnidadesFaenas.GetUnidadFaena
{
    public class GetUnidadFaenaHandler : IRequestHandler<GetUnidadFaenaRequest, GetUnidadFaenaResponse>
    {
        private readonly MeatContext context;

        public GetUnidadFaenaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetUnidadFaenaResponse> Handle(GetUnidadFaenaRequest request, CancellationToken cancellationToken)
        {
            return await (
                from u in this.context.UnidadesFaenas
                join e in this.context.Especies on u.EspecieId equals e.Codigo into ej
                from e in ej.DefaultIfEmpty()
                where u.Codigo == request.Codigo
                select new GetUnidadFaenaResponse
                {
                    Codigo = u.Codigo,
                    EspecieId = u.EspecieId,
                    EspecieNombre = e != null ? e.Nombre : null,
                    Nombre = u.Nombre,
                    CantidadCuartos = u.CantidadCuartos,
                    PiezasPorAnimal = u.PiezasPorAnimal,
                    PorDefecto = u.PorDefecto,
                    CodigoMaterial = u.CodigoMaterial,
                    ERP_Codigo = u.ERP_Codigo,
                    Activo = u.Activo
                }
            ).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
