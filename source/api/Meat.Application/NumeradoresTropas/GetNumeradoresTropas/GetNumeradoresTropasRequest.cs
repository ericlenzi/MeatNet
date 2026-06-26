using MediatR;

namespace Meat.Application.NumeradoresTropas.GetNumeradoresTropas
{
    public class GetNumeradoresTropasRequest : IRequest<GetNumeradoresTropasResponse>
    {
        public string Filter { get; set; }
    }
}
