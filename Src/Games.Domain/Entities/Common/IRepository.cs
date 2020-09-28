using System;
using System.Threading;
using System.Threading.Tasks;

namespace Games.Domain.Entities.Common
{
    public interface IRepository<TEntity> where TEntity : IAggregateRoot
    {
        Task<TEntity> FindAsync(Guid id, CancellationToken cancellationToken);

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        IUnitOfWork UnitOfWork { get; }
    }
}
