using System;
using System.Collections.Generic;
using System.Text;

namespace Meat.Queries.Dtos.Response
{
    public class UsuariosResponse
    {
        public int TotalRows { get; set; }
        public IEnumerable<UsuarioDto> Usuarios { get; set; }
    }
}
