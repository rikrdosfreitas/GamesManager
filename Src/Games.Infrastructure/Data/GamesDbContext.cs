using System.Threading;
using System.Threading.Tasks;
using Games.Domain.Entities.Common;
using Games.Domain.Entities.FriendAggregate;
using Games.Domain.Entities.GameAggregate;
using Microsoft.EntityFrameworkCore;

namespace Games.Infrastructure.Data
{
    public class GamesDbContext : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "dbo";

        protected GamesDbContext()
        {
        }

        public GamesDbContext(DbContextOptions<GamesDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Friend> Friends { get; set; }

        public virtual DbSet<Game> Games { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DEFAULT_SCHEMA);

            GamesDbContextEntities
                 .Build(this)
                .Configure(modelBuilder);
        }

        public Task<bool> SaveAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        public virtual void SetModified(object entity)
        {
            this.Entry(entity).State = EntityState.Modified; 
        }
    }
}