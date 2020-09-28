using Microsoft.EntityFrameworkCore;

namespace Games.Common.Transaction.Utilities
{
    public interface IDbContextRegistry
    {
        IDbContextManager RegisterContext(DbContext context);
    }
}