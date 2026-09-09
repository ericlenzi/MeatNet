namespace Meat.Application.Shared.GeneratePassword
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class GeneratePasswordHandler : IRequestHandler<GeneratePasswordRequest, GeneratePasswordResponse>
    {
        public Task<GeneratePasswordResponse> Handle(GeneratePasswordRequest request, CancellationToken cancellationToken)
        {
            var newPassword = request.Contraseña;

            return Task.FromResult(new GeneratePasswordResponse()
            {
                Password = newPassword,
                PasswordHash = PasswordHash.Calcular(newPassword),
            });
        }
    }
}