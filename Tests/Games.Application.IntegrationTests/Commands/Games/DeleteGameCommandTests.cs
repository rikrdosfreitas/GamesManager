using System.Collections.Generic;
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

    [TestFixture]
    public class DeleteGameCommandTests : TestBase
    {
        private Game _entity;

        [SetUp]
        protected void SetUp()
        {
            _entity = Game.Create(SequentialGuid.NewGuid(), "Bugsnax", 2020, "PS 4");

            var list = new List<Game>
            {
                Game.Create(SequentialGuid.NewGuid(),"Call of Duty: Black Ops Cold War" ,2020, "X-Box"),
                Game.Create(SequentialGuid.NewGuid(),"Immortals Fenyx Rising" ,2020, "PS 5")
            };

            list.Add(_entity);

            using var scope = _scopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<GamesDbContext>();

            context.Games.AddRange(list);

            context.SaveChanges();
        }

        [Test]
        public async Task ShouldDeleteGame()
        {
            var entity = await FindAsync<IGameRepository, Game>(_entity.Id);
            entity.Should().NotBeNull();
            entity.Name.Should().Be("Bugsnax");

            var cmd = new DeleteGameCommand(_entity.Id);

            await SendAsync(cmd);

            entity = await FindAsync<IGameRepository, Game>(_entity.Id);
            entity.Should().BeNull();
        }


    }
}