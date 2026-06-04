using System.Collections.Generic;
using Meat.Domain.TiposEmpresas;

namespace Meat.Application.TiposEmpresas.GetTiposEmpresas
{
    public class GetTiposEmpresasResponse
    {
        public IEnumerable<TipoEmpresa> Data { get; set; }
    }
}
