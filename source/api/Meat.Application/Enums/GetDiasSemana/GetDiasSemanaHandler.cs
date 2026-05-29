namespace Meat.Application.Enums.GetSexos
{
    using MediatR;
    using Meat.Application.Enums.GetSexos;
    using Meat.Domain.Enums;
    using System.Threading;
    using System.Threading.Tasks;

    public class GetDiasSemanaHandler : IRequestHandler<GetDiasSemanaRequest, GetDiasSemanaResponse>
    {
        public GetDiasSemanaHandler()
        {
        }

        public async Task<GetDiasSemanaResponse> Handle(GetDiasSemanaRequest request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(
                new GetDiasSemanaResponse
                {
                    Data = EnumDtoBuilder.EnumToEnumDto<DiaSemanaEnum>(),
                }
            );
        }
    }
}
