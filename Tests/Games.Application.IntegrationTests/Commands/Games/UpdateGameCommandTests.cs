using System.Threading.Tasks;
using FluentAssertions;
using Games.Application.Commands.Games;
using Games.Common.Util;
using Games.Domain.Entities.GameAggregate;
using Games.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Games.Application.IntegrationTests.Commands.Games
{
    using static Testing;

    [TestFixture()]
    public class UpdateGameCommandTests
    {
        private Game _entity;

        [SetUp]
        protected void SetUp()
        {
            _entity = Game.Create(SequentialGuid.NewGuid(), "Call of Duty: Black Ops Cold", 2010, "X-Box");


            using var scope = _scopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<GamesDbContext>();

            context.Games.Add(_entity);

            context.SaveChanges();
        }

        [Test]
        public async Task ShouldUpdateGame()
        {
            var cmd = new UpdateGameCommand(_entity.Id, "Call of Duty: Black Ops Cold War",2020, "X-Box", _entity.Ver);

            await SendAsync(cmd);

            var entity = await FindAsync<IGameRepository, Game>(_entity.Id);

            entity.Should().NotBeNull();
            entity.Id.Should().Be(_entity.Id);
            entity.Name.Should().Be("Call of Duty: Black Ops Cold War");
            entity.LaunchYear.Should().Be(2020);
            entity.Platform.Should().Be("X-Box");
        }
    }
}