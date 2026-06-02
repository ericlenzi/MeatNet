using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Domain.Usuarios;
using Meat.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Usuarios.GetUsuarios
{
    public class GetUsuariosHandler : IRequestHandler<GetUsuariosRequest, GetUsuariosResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public GetUsuariosHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<GetUsuariosResponse> Handle(GetUsuariosRequest request, CancellationToken cancellationToken)
        {
            IQueryable<Usuario> queryable = this.context.Usuarios
                .Include(x => x.Empresa)
                .Where(x => x.Empresa.CodigoEmpresa == request.CodigoEmpresa)
                .AsQueryable();

            if (request.Estado.HasValue)
                queryable = queryable.Where(x => x.Activo == (request.Estado.Value == 1));

            if (!string.IsNullOrEmpty(request.Rol))
                queryable = queryable.Where(x => x.RolId == request.Rol);

            if (!string.IsNullOrEmpty(request.Filter))
                queryable = queryable.Where(x =>
                    x.Nombre.Contains(request.Filter) ||
                    x.Apellido.Contains(request.Filter) ||
                    x.UserName.Contains(request.Filter) ||
                    x.Email.Contains(request.Filter) ||
                    x.Legajo.Contains(request.Filter));

            queryable = queryable.OrderBy(x => x.Apellido).ThenBy(x => x.Nombre);

            var totalRows = await queryable.CountAsync(cancellationToken);

            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync(cancellationToken);

            return new GetUsuariosResponse()
            {
                Data = this.mapper.Map<IEnumerable<GetUsuariosItem>>(data),
                TotalRows = totalRows
            };
        }
    }
}
