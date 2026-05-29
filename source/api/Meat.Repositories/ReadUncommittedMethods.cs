using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Meat.Repositories
{
    public static class ReadUncommittedMethods
    {
        public static async Task<T> FirstWithNoLock<T>(this IQueryable<T> query)
        {
            using var scope = CreateTransaction();

            T toReturn = await query.FirstOrDefaultAsync();
            scope.Complete();
            return toReturn;
        }

        public static async Task<List<T>> ToListWithNoLock<T>(this IQueryable<T> query)
        {
            using var scope = CreateTransaction();

            List<T> toReturn = await query.ToListAsync();
            scope.Complete();
            return toReturn;
        }

        private static TransactionScope CreateTransaction()
        {
            return new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions()
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted
                },
                TransactionScopeAsyncFlowOption.Enabled);
        }
    }
}