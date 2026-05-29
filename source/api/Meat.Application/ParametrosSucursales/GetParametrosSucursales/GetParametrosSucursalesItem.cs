using System;

namespace Meat.Application.ParametrosSucursales.GetParametrosSucursales
{
    public class GetParametrosSucursalesItem
    {
        public Guid Id { get; set; }
        public Guid ParametroId { get; set; }
        public Guid SucursalId { get; set; }
        public string Codigo { get; set; }
        public string Valor { get; set; }
    }
}
