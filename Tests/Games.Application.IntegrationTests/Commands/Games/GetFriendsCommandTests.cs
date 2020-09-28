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
    public class GetGamesCommandTests : TestBase
    {
        [SetUp]
        protected void SetUp()
        {
            var list = new List<Game>
            {
                Game.Create(SequentialGuid.NewGuid(),"Call of Duty: Black Ops Cold War" ,2020, "X-Box"),
                Game.Create(SequentialGuid.NewGuid(),"Immortals Fenyx Rising" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Ratchet & Clank: Rift Apart" ,2015, "PC"),
                Game.Create(SequentialGuid.NewGuid(),"Bridge Constructor: The Walking Dead" ,2020, "PS 2"),
                Game.Create(SequentialGuid.NewGuid(),"Bugsnax" ,2020, "PS 4"),
                Game.Create(SequentialGuid.NewGuid(),"Watch Dogs: Legion" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Assassin's Creed Valhalla" ,2020, "PC")
            };

            using var scope = _scopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<GamesDbContext>();

            context.Games.AddRange(list);

            context.SaveChanges();
        }


        [Test]
        public async Task ShouldReturnGames()
        {
            var result = await SendAsync(new GetGamesCommand { Search = "", Sort = "name", Order = "asc", Page = 0, Size = 5 });

            result.Records.Should().Be(7);
            result.Data.Should()
                .HaveCount(5)
                .And
                .AllBeAssignableTo<GameListViewModel>();
        }
    }
}