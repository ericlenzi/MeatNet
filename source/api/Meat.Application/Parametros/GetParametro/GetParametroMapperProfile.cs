using Meat.Domain.Parametros;

namespace Meat.Application.Parametros.GetParametro
{
    public class GetParametroMapperProfile : AutoMapper.Profile
    {
        public GetParametroMapperProfile()
        {
            this.CreateMap<Parametro, GetParametroResponse>();
        }
    }
}
