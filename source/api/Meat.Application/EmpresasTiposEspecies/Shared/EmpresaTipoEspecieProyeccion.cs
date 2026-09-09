using Meat.Application.EmpresasTiposEspecies.GetEmpresasTiposEspecies;
using Meat.Domain.EmpresasTiposEspecies;
using System;
using System.Linq.Expressions;

namespace Meat.Application.EmpresasTiposEspecies.Shared
{
    /// <summary>
    /// Como se aplana la configuracion de la empresa junto con los datos del catalogo global.
    /// Vive aca para que el listado y el detalle no se separen.
    /// </summary>
    public static class EmpresaTipoEspecieProyeccion
    {
        public static readonly Expression<Func<EmpresaTipoEspecie, GetEmpresasTiposEspeciesItem>> Item =
            x => new GetEmpresasTiposEspeciesItem
            {
                Id = x.Id,
                TipoEspecieId = x.TipoEspecieId,
                Nombre = x.TipoEspecie.Nombre,
                EspecieId = x.TipoEspecie.EspecieId,
                EspecieNombre = x.TipoEspecie.Especie != null ? x.TipoEspecie.Especie.Nombre : string.Empty,
                TipoSexoId = x.TipoEspecie.TipoSexoId,
                TipoSexoNombre = x.TipoEspecie.TipoSexo != null ? x.TipoEspecie.TipoSexo.Nombre : string.Empty,
                PesoTeorico = x.PesoTeorico,
                PesoTeoricoReferencia = x.TipoEspecie.PesoTeoricoReferencia,
                ERP_Codigo = x.ERP_Codigo,
                Activo = x.Activo,
                TipoEspecieActivo = x.TipoEspecie.Activo,
            };
    }
}
