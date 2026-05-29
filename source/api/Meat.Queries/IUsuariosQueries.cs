using Meat.Queries.Dtos.Response;
using System.Threading.Tasks;

namespace Meat.Queries
{
    public interface IUsuariosQueries
    {
        Task<UsuariosResponse> GetUsuariosAsync(int? rol, int? estado, string filter, int pageSize, int pageIndex);
    }
}
