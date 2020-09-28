using System;
using System.Threading;
using System.Threading.Tasks;
using Games.Domain.Entities.Common;

namespace Games.Domain.Entities.GameAggregate
{
    public interface IGameQuery : IQuery<Game>
    {
        Task<Game> GetLoanedInfo(Guid requestId, CancellationToken cancellationToken);
    }
}