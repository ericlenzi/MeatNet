using System;

namespace Meat.Domain.Sucursales
{
    public static class SucursalFactory
    {
        public static Sucursal Create()
        {
            return new Sucursal()
            {
                Id = Guid.NewGuid(),
                FechaActualizacion = DateTime.Now
            };
        }

        public static Sucursal Create(string nombre, Guid empresaId)
        {
            return new Sucursal()
            {
                Id = Guid.NewGuid(),
                Nombre = nombre,
                EmpresaId = empresaId,
                FechaActualizacion = DateTime.Now,
                Activo = true
            };
        }
    }
}
