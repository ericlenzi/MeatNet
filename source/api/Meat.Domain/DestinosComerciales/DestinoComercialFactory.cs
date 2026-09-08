using System;

namespace Meat.Domain.DestinosComerciales
{
    public static class DestinoComercialFactory
    {
        public static DestinoComercial Create()
        {
            return new DestinoComercial
            {
                Id = Guid.NewGuid(),
                Activo = true
            };
        }
    }
}
