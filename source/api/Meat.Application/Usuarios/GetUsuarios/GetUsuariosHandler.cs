using AutoMapper;
using MediatR;
using Meat.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Usuarios.GetUsuarios
{

    public class GetUsuariosHandler : IRequestHandler<GetUsuariosRequest, GetUsuariosResponse>
    {
        private readonly IMapper mapper;
        private readonly IUsuariosQueries usuariosQueries;

        public GetUsuariosHandler(IMapper mapper, IUsuariosQueries usuariosQueries)
        {
            this.mapper = mapper;
            this.usuariosQueries = usuariosQueries;
        }

        public async Task<GetUsuariosResponse> Handle(GetUsuariosRequest request, CancellationToken cancellationToken)
        {

            var data = await this.usuariosQueries.GetUsuariosAsync(request.Rol, request.Estado, request.Filter, request.PageSize, request.PageIndex);

            return new GetUsuariosResponse()
            {
                Data = this.mapper.Map<IEnumerable<GetUsuariosItem>>(data.Usuarios),
                TotalRows = data.TotalRows,
            }; 
        }
    }
}
