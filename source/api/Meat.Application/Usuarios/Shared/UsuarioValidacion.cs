using Meat.Application.Shared;
using System;

namespace Meat.Application.Usuarios.Shared
{
    /// <summary>
    /// Lo que tienen que cumplir los datos de un usuario, en el alta y en la edicion.
    /// </summary>
    public static class UsuarioValidacion
    {
        /// <summary>
        /// Rol de la empresa administrativa: administra el padron de empresas y los catalogos globales
        /// de todas. No se asigna desde el ABM de usuarios de una empresa; si se pudiera, el ADMIN de
        /// cualquier empresa se daria acceso a las demas.
        /// </summary>
        public const string RolSuperAdmin = "SUPERADMIN";

        public static void ValidarRolAsignable(string rolId)
        {
            if (string.Equals(rolId?.Trim(), RolSuperAdmin, StringComparison.OrdinalIgnoreCase))
                throw new ValidationException("El rol SUPERADMIN no se puede asignar desde la administracion de usuarios.");
        }
    }
}
