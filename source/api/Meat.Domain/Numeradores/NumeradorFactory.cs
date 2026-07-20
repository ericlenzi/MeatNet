using System;

namespace Meat.Domain.Numeradores
{
    public static class NumeradorFactory
    {
        public static Numerador Create()
        {
            return new Numerador()
            {
                Id = Guid.NewGuid(),
                FechaActualizacion = DateTime.Now
            };
        }
    }
}
