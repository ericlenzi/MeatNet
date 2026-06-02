using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Domain.Clientes;
using Meat.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Clientes.GetClientes
{
    public class GetClientesHandler : IRequestHandler<GetClientesRequest, GetClientesResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public GetClientesHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<GetClientesResponse> Handle(GetClientesRequest request, CancellationToken cancellationToken)
        {
            IQueryable<Cliente> queryable = this.context.Clientes
                .Include(x => x.EmpresaPadre)
                .Include(x => x.TipoCliente)
                .Where(x => x.EmpresaPadre.CodigoEmpresa == request.CodigoEmpresa);

            if (request.Estado.HasValue)
                queryable = queryable.Where(x => x.Activo == request.Estado.Value);

            if (!string.IsNullOrEmpty(request.Filter))
                queryable = queryable.Where(x =>
                    x.CodigoCliente.Contains(request.Filter) ||
                    x.Nombre.Contains(request.Filter) ||
                    x.NumeroCuit.Contains(request.Filter));

            queryable = queryable.OrderBy(x => x.CodigoCliente);

            var totalRows = await queryable.CountAsync();

            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync();

            return new GetClientesResponse()
            {
                Data = this.mapper.Map<IEnumerable<GetClientesItem>>(data),
                TotalRows = totalRows,
            };
        }
    }
}
