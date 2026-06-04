using System;

namespace Meat.Application.Autenticacion
{
    public class LoginResponse
    {
        public CurrentUser CurrentUser { get; set; }
        public string Token { get; set; }
        public bool DebeCambiarContrasena { get; set; }
    }

    public class CurrentUser
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string NombreCompleto { get; set; }
        public string RolId { get; set; }
        public string CodigoEmpresa { get; set; }
        public string NombreEmpresa { get; set; }
        public string CodigoSucursal { get; set; }
    }
}
