using System;

namespace Meat.Domain.Tipificadores
{
    public static class TipificadorFactory
    {
        public static Tipificador Create()
        {
            return new Tipificador()
            {
                Id = Guid.NewGuid(),
                Activo = true,
                FechaActualizacion = DateTime.Now
            };
        }
    }
}
