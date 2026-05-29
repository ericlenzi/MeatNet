using Meat.Domain.ParametrosSucursales;

namespace Meat.Application.ParametrosSucursales.GetParametrosSucursales
{
    public class GetParametrosSucursalesMapper : AutoMapper.Profile
    {
        public GetParametrosSucursalesMapper()
        {
            this.CreateMap<ParametroSucursal, GetParametrosSucursalesItem>()
                    .ForMember(d => d.Codigo, c => c.MapFrom(s => s.Parametro.Codigo));
        }
    }
}
