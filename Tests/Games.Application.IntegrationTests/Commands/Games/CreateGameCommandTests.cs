using System.Threading.Tasks;
using FluentAssertions;
using Games.Application.Commands.Games;
using Games.Domain.Entities.GameAggregate;
using NUnit.Framework;

namespace Games.Application.IntegrationTests.Commands.Games
{
    using static Testing;

    [TestFixture()]
    public class CreateGameCommandTests : TestBase
    {
        [Test]
        public async Task ShouldCreateGame()
        {
            var cmd = new CreateGameCommand("Ratchet & Clank: Rift Apart", 2015, "PC");

            await SendAsync(cmd);

            var entity = await FindAsync<IGameRepository, Game>(cmd.Id);
            entity.Should().NotBeNull();
            entity.Id.Should().Be(cmd.Id);
            entity.Name.Should().Be("Ratchet & Clank: Rift Apart");
            entity.LaunchYear.Should().Be(2015);
            entity.Platform.Should().Be("PC");
        }
    }
}
