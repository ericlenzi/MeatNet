using System;

namespace Meat.Domain.DespiecesMateriales
{
    public static class DespieceMaterialFactory
    {
        public static DespieceMaterial Create()
        {
            return new DespieceMaterial
            {
                Id = Guid.NewGuid(),
                Activo = true
            };
        }
    }
}
