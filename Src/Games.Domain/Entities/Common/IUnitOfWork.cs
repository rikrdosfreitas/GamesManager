using System.Threading;
using System.Threading.Tasks;

namespace Games.Domain.Entities.Common
{
    public interface IUnitOfWork
    {
        Task<bool> SaveAsync(CancellationToken cancellationToken = default);
    }
}