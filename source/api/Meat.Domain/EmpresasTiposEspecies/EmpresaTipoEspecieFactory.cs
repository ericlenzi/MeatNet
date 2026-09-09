using System;

namespace Meat.Domain.EmpresasTiposEspecies
{
    public static class EmpresaTipoEspecieFactory
    {
        public static EmpresaTipoEspecie Create()
        {
            return new EmpresaTipoEspecie
            {
                Id = Guid.NewGuid(),
                FechaActualizacion = DateTime.Now,
                Activo = true
            };
        }
    }
}
