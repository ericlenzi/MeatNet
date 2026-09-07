using System;

namespace Meat.Domain.Materiales
{
    public static class MaterialFactory
    {
        public static Material Create()
        {
            return new Material
            {
                Id = Guid.NewGuid(),
                Activo = true
            };
        }
    }
}
