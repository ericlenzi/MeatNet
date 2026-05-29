using System;

namespace Meat.Domain.ParametrosSucursales
{
    public static class ParametroSucursalFactory
    {
        public static ParametroSucursal Create(Guid sucursalId, Guid parametroId, string valor)
        {
            return new ParametroSucursal()
            {
                Id = Guid.NewGuid(),
                ParametroId = parametroId,
                SucursalId = sucursalId,
                Valor = valor,
                FechaActualizacion = DateTime.Now
            };
        }
    }
}
