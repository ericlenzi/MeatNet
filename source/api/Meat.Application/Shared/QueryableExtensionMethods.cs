using System.Linq;

namespace Meat.Application.Shared
{
    public static class QueryableExtensionMethods
    {
        public static IQueryable<T> Page<T>(this IQueryable<T> queryable, int pageSize, int pageIndex)
        {
            return pageSize > 0 ? queryable.Skip(pageIndex * pageSize).Take(pageSize) : queryable;
        }
    }
}