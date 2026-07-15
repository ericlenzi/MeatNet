using System;

namespace Meat.Domain.ListasMatanzas
{
    public static class ListaMatanzaFactory
    {
        public static ListaMatanza Create()
        {
            return new ListaMatanza()
            {
                Id = Guid.NewGuid(),
                Version = 0,
                FechaActualizacion = DateTime.Now
            };
        }
    }
}
