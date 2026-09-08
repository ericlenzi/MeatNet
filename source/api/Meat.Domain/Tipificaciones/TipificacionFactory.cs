using System;

namespace Meat.Domain.Tipificaciones
{
    public static class TipificacionFactory
    {
        public static Tipificacion Create()
        {
            return new Tipificacion
            {
                Id = Guid.NewGuid(),
                FechaActualizacion = DateTime.Now,
                Activo = true
            };
        }
    }
}
