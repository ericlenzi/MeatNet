using MediatR;

namespace Meat.Application.TiposMediciones.GetTipoMedicion
{
    public class GetTipoMedicionRequest : IRequest<GetTipoMedicionResponse>
    {
        public string Codigo { get; set; }
    }
}
