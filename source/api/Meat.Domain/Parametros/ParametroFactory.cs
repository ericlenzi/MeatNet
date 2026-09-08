using System;

namespace Meat.Domain.Parametros
{
    public static class ParametroFactory
    {
        public static Parametro Create()
        {
            return new Parametro
            {
                Id = Guid.NewGuid(),
                Activo = true
            };
        }
    }
}
