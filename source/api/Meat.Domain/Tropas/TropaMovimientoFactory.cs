using System;

namespace Meat.Domain.Tropas
{
    public static class TropaMovimientoFactory
    {
        public static TropaMovimiento Create()
        {
            return new TropaMovimiento()
            {
                Id = Guid.NewGuid()
            };
        }
    }
}
