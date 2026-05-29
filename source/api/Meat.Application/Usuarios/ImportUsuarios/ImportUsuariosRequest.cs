using System.Collections.Generic;
using MediatR;

namespace Meat.Application.Usuarios.ImportUsuarios
{
    public class ImportUsuariosRequest : IRequest<ImportUsuariosResponse>
    {
        public IEnumerable<ImportUsuariosItem> Data { get; set; }
    }
}
