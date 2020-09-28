using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Games.Domain.Entities.Common
{
    public interface IQuery<TEntity> where TEntity : IAggregateRoot
    {
        Task<TEntity> GetAsync(Guid id, CancellationToken cancellationToken);

        IQueryable<TEntity> GetAllAsync(string search, string sort, string order, int page, int size);

        Task<int> CountAsync(string search, CancellationToken cancellationToken);
    }
}