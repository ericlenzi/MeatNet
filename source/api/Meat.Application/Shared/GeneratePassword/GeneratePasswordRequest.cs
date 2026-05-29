namespace Meat.Application.Shared.GeneratePassword
{
    using MediatR;

    public class GeneratePasswordRequest : IRequest<GeneratePasswordResponse>
    {
        public string Contraseña { get; set; }
    }
}