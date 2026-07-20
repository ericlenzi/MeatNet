using System;

namespace Meat.Domain.Romaneos
{
    public static class RomaneoFactory
    {
        public static Romaneo Create()
        {
            return new Romaneo()
            {
                Id = Guid.NewGuid(),
                Fecha = DateTime.Now,
                Anulado = false
            };
        }

        public static RomaneoPieza CreatePieza()
        {
            return new RomaneoPieza()
            {
                Id = Guid.NewGuid()
            };
        }

        public static RomaneoPiezaMedicion CreateMedicion()
        {
            return new RomaneoPiezaMedicion()
            {
                Id = Guid.NewGuid()
            };
        }
    }
}
