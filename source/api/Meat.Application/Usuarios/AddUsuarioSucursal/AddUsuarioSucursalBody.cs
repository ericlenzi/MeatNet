using System;

namespace Meat.Application.Usuarios.AddUsuarioSucursal
{
    public class AddUsuarioSucursalBody
    {
        public Guid SucursalId { get; set; }
        public bool EsMain { get; set; }
    }
}
