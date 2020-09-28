using System;
using System.Threading;
using System.Threading.Tasks;
using Games.Domain.Entities.Common;
using Games.Domain.Entities.GameAggregate;
using Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Games.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly GamesDbContext _context;

        public GameRepository(GamesDbContext context)
        {
            _context = context;
            UnitOfWork = _context;
        }

        public IUnitOfWork UnitOfWork { get; }

        public Task<Game> FindAsync(Guid id, CancellationToken cancellationToken)
        {
            return _context.Games.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public void Add(Game entity)
        {
            _context.Games.Add(entity);
        }

        public void Update(Game entity)
        {
            _context.SetModified(entity);
        }

        public void Delete(Game entity)
        {
            _context.Games.Remove(entity);
        }
    }
}
