using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Usuarios.GetUsuarioSucursales
{
    public class GetUsuarioSucursalesHandler : IRequestHandler<GetUsuarioSucursalesRequest, GetUsuarioSucursalesResponse>
    {
        private readonly MeatContext context;

        public GetUsuarioSucursalesHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetUsuarioSucursalesResponse> Handle(GetUsuarioSucursalesRequest request, CancellationToken cancellationToken)
        {
            var usuarioSucursales = await this.context.UsuariosSucursales
                .Where(us => us.UsuarioId == request.UsuarioId)
                .Join(
                    this.context.Sucursales
                        .Where(s => s.Empresa.CodigoEmpresa == request.CodigoEmpresa),
                    us => us.SucursalId,
                    s => s.Id,
                    (us, s) => new UsuarioSucursalItem
                    {
                        Id = us.Id,
                        SucursalId = s.Id,
                        CodigoSucursal = s.CodigoSucursal,
                        Nombre = s.Nombre,
                        Color = s.Color,
                        EsMain = us.EsMain
                    })
                .OrderBy(x => x.Nombre)
                .ToListAsync(cancellationToken);

            return new GetUsuarioSucursalesResponse
            {
                Data = usuarioSucursales
            };
        }
    }
}
