using System.Threading.Tasks;

namespace Meat.Infrastructure.Services
{
    public interface IFtpService
    {
        Task<bool> Send(string filePath);
        Task<bool> SendSyncMateriales(string filePath);
        Task<bool> SendSyncVentas(string filePath);
    }
}
