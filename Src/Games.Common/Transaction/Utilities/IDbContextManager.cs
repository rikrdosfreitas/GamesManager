using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Games.Common.Transaction.Utilities
{
    public interface IDbContextManager : IDbContextRegistry
    {
        bool IsInTransaction { get; }

        Task ExecuteInTransactionAsync(DbContext dbContext, Func<CancellationToken, Task> operation, CancellationToken token = default);
    }
}
