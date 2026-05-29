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
            using (SHA1 sha1Hash = SHA1.Create())
            {
                var newPassword = request.Contraseña;
                byte[] sourceBytes = Encoding.UTF8.GetBytes(newPassword);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                string passwordHash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

                return Task.FromResult(new GeneratePasswordResponse()
                {
                    Password = newPassword,
                    PasswordHash = passwordHash,
                });
            }
        }
    }
}