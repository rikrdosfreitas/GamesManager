using System;
using System.Threading;
using System.Threading.Tasks;
using Games.Domain.Entities.Common;

namespace Games.Domain.Entities.FriendAggregate
{
    public interface IFriendQuery : IQuery<Friend>
    {
        Task<bool> IsValid(Guid id, CancellationToken cancellationToken);
    }
}