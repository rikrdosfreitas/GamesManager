using Games.Domain.Entities.Common;
using Games.Domain.Entities.FriendAggregate;
using Microsoft.EntityFrameworkCore;
using Games.Domain.Entities.GameAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Games.Infrastructure.Data
{
    public class GamesDbContextEntities
    {
        public static GamesDbContextEntities Build(GamesDbContext context)
        {
            return new GamesDbContextEntities();
        }

        public void Configure(ModelBuilder optionsBuilder)
        {

            foreach (var entityType in optionsBuilder.Model.GetEntityTypes())
            {
                if (typeof(IAggregateRoot).IsAssignableFrom(entityType.ClrType))
                {
                    optionsBuilder.Entity(entityType.ClrType)
                        .HasKey(nameof(IAggregateRoot.Id));

                    optionsBuilder.Entity(entityType.ClrType)
                        .Property(nameof(IAggregateRoot.Id))
                        .ValueGeneratedNever();
                }
            }

            optionsBuilder.Entity<Friend>(Configure);
            optionsBuilder.Entity<Game>(Configure);
        }

        private void Configure(EntityTypeBuilder<Friend> entry)
        {
            
        }

        private void Configure(EntityTypeBuilder<Game> entry)
        {
            entry
                .OwnsOne(x => x.Loan); ;

            var converter = new EnumToStringConverter<GameState>();
            entry
                .Property(e => e.State)
                .HasConversion(converter)
                .HasMaxLength(30);
        }


    }
}
