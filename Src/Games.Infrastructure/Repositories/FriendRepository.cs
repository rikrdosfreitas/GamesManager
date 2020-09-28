using System;
using System.Threading;
using System.Threading.Tasks;
using Games.Domain.Entities.Common;
using Games.Domain.Entities.FriendAggregate;
using Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Games.Infrastructure.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        private readonly GamesDbContext _context;

        public FriendRepository(GamesDbContext context)
        {
            _context = context;
            UnitOfWork = _context;
        }

        public IUnitOfWork UnitOfWork { get; }

        public Task<Friend> FindAsync(Guid id, CancellationToken cancellationToken)
        {
            return _context.Friends.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public void Add(Friend entity)
        {
            _context.Friends.Add(entity);
        }

        public void Update(Friend entity)
        {
            _context.SetModified(entity);
        }

        public void Delete(Friend entity)
        {
            _context.Friends.Remove(entity);
        }

    }
}
