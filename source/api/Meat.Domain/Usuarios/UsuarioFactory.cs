using System;

namespace Meat.Domain.Usuarios
{
    public static class UsuarioFactory
    {
        public static Usuario Create(
            string userName,
            string passwordHash,
            string nombre,
            string apellido,
            string email,
            string legajo,
            string rolId,
            Guid empresaId,
            bool activo
        )
        {
            return new Usuario()
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                PasswordHash = passwordHash,
                Nombre = nombre,
                Apellido = apellido,
                Email = email,
                Legajo = legajo,
                RolId = rolId,
                EmpresaId = empresaId,
                FechaActualizacion = DateTime.Now,
                Activo = activo
            };
        }
    }
}