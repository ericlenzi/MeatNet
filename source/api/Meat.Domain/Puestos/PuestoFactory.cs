using System;

namespace Meat.Domain.Puestos
{
    public static class PuestoFactory
    {
        public static Puesto Create()
        {
            return new Puesto()
            {
                Id = Guid.NewGuid(),
                Activo = true,
                FechaActualizacion = DateTime.Now
            };
        }
    }
}
