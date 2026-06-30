using System;

namespace Meat.Domain.Tropas
{
    public static class TropaFactory
    {
        public static Tropa Create()
        {
            return new Tropa()
            {
                Id = Guid.NewGuid()
            };
        }
    }
}
