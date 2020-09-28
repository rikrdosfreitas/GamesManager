using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Games.Application.Commands.Games;
using Games.Application.Models.Games;
using Games.Common.Util;
using Games.Domain.Entities.GameAggregate;
using Games.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Games.Application.IntegrationTests.Commands.Games
{
    using static Testing;

    [TestFixture()]
    public class GetGameCommandTests : TestBase
    {
        private Game _entity;

        [SetUp]
        protected void SetUp()
        {
            _entity = Game.Create(SequentialGuid.NewGuid(), "Ratchet & Clank: Rift Apart", 2015, "PC");

            var list = new List<Game>
            {
                Game.Create(SequentialGuid.NewGuid(),"Call of Duty: Black Ops Cold War" ,2020, "X-Box"),
                Game.Create(SequentialGuid.NewGuid(),"Immortals Fenyx Rising" ,2020, "PS 5"),
                
            };

            list.Add(_entity);

            using var scope = _scopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<GamesDbContext>();

            context.Games.AddRange(list);

            context.SaveChanges();
        }

        [Test]
        public async Task ShouldReturnGames()
        {
           var result = await SendAsync(new GetGameCommand(_entity.Id));

            result.Should().NotBeNull();
            result.Id.Should().Be(_entity.Id);
            result.Name.Should().Be("Ratchet & Clank: Rift Apart");
            result.LaunchYear.Should().Be(2015);
            result.Platform.Should().Be("PC");
            result.Should().BeAssignableTo<GameViewModel>();
        }
    }
}