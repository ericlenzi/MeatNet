using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Usuarios.GetUsuarioEstablecimientos
{
    public class GetUsuarioEstablecimientosHandler : IRequestHandler<GetUsuarioEstablecimientosRequest, GetUsuarioEstablecimientosResponse>
    {
        private readonly MeatContext context;

        public GetUsuarioEstablecimientosHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetUsuarioEstablecimientosResponse> Handle(GetUsuarioEstablecimientosRequest request, CancellationToken cancellationToken)
        {
            var items = await this.context.UsuariosEstablecimientos
                .Where(ue => ue.UsuarioId == request.UsuarioId)
                .Join(
                    this.context.Establecimientos
                        .Include(e => e.Sucursal)
                        .Where(e => e.Empresa.CodigoEmpresa == request.CodigoEmpresa),
                    ue => ue.EstablecimientoId,
                    e => e.Id,
                    (ue, e) => new UsuarioEstablecimientoItem
                    {
                        Id = ue.Id,
                        EstablecimientoId = e.Id,
                        CodigoEstablecimiento = e.CodigoEstablecimiento,
                        Nombre = e.Nombre,
                        SucursalId = e.SucursalId,
                        CodigoSucursal = e.Sucursal.CodigoSucursal,
                        NombreSucursal = e.Sucursal.Nombre,
                        EsMain = ue.EsMain
                    })
                .OrderBy(x => x.NombreSucursal)
                .ThenBy(x => x.Nombre)
                .ToListAsync(cancellationToken);

            return new GetUsuarioEstablecimientosResponse { Data = items };
        }
    }
}
