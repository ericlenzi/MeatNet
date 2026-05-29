namespace Meat.Application.Enums.GetSexos
{
    using MediatR;
    using Meat.Domain.Enums;
    using System.Threading;
    using System.Threading.Tasks;

    public class GetSexosHandler : IRequestHandler<GetSexosRequest, GetSexosResponse>
    {
        public GetSexosHandler()
        {
        }

        public async Task<GetSexosResponse> Handle(GetSexosRequest request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(
                new GetSexosResponse
                {
                    Data = EnumDtoBuilder.EnumToEnumDto<SexoEnum>(),
                }
            );
        }
    }
}