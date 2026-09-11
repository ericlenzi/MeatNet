using MediatR;

namespace Meat.Application.TiposMediciones.DeleteTipoMedicion
{
    public class DeleteTipoMedicionRequest : IRequest<DeleteTipoMedicionResponse>
    {
        public string Codigo { get; set; }
    }
}
