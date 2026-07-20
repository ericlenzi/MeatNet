using System;

namespace Meat.Domain.UnidadesFaenas
{
    public static class UnidadFaenaFactory
    {
        public static UnidadFaena Create()
        {
            return new UnidadFaena()
            {
                Id = Guid.NewGuid(),
                FechaActualizacion = DateTime.Now
            };
        }
    }
}
