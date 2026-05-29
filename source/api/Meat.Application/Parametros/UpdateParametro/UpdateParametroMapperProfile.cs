using AutoMapper;
using Meat.Domain.Parametros;

namespace Meat.Application.Parametros.UpdateParametro
{
    public class UpdateParametroMapperProfile : Profile
    {
        public UpdateParametroMapperProfile()
        {
            this.CreateMap<UpdateParametroRequest, Parametro>();
        }
    }
}
