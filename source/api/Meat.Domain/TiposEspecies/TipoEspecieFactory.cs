using System;

namespace Meat.Domain.TiposEspecies
{
    public static class TipoEspecieFactory
    {
        public static TipoEspecie Create()
        {
            return new TipoEspecie
            {
                Id = Guid.NewGuid(),
                FechaActualizacion = DateTime.Now,
                Activo = true
            };
        }
    }
}
