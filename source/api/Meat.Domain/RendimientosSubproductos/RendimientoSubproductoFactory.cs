using System;

namespace Meat.Domain.RendimientosSubproductos
{
    public static class RendimientoSubproductoFactory
    {
        public static RendimientoSubproducto Create()
        {
            return new RendimientoSubproducto()
            {
                Id = Guid.NewGuid(),
                Activo = true,
                FechaActualizacion = DateTime.Now
            };
        }
    }
}
