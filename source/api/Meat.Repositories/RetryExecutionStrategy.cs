using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace Meat.Repositories
{
    public class RetryExecutionStrategy : ExecutionStrategy
    {
        public RetryExecutionStrategy(DbContext context, int maxRetryCount, TimeSpan maxRetryDelay) :
            base(context, maxRetryCount, maxRetryDelay)
        {
        }

        protected override bool ShouldRetryOn(Exception exception)
        {
            return exception.GetType() == typeof(InvalidOperationException) || exception.GetType() == typeof(SqlException) || exception.GetType() == typeof(DbUpdateException);
        }
    }
}