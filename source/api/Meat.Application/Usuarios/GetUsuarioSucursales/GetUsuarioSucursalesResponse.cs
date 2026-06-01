using System;
using System.Collections.Generic;

namespace Meat.Application.Usuarios.GetUsuarioSucursales
{
    public class GetUsuarioSucursalesResponse
    {
        public IEnumerable<UsuarioSucursalItem> Data { get; set; }
    }

    public class UsuarioSucursalItem
    {
        public Guid Id { get; set; }
        public Guid SucursalId { get; set; }
        public string CodigoSucursal { get; set; }
        public string Nombre { get; set; }
        public string Color { get; set; }
        public bool EsMain { get; set; }
    }
}
