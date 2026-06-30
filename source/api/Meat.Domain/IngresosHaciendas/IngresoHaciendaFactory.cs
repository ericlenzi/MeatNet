using System;

namespace Meat.Domain.IngresosHaciendas
{
    public static class IngresoHaciendaFactory
    {
        public static IngresoHacienda Create()
        {
            return new IngresoHacienda()
            {
                Id = Guid.NewGuid(),
                FechaActualizacion = DateTime.Now
            };
        }
    }
}
