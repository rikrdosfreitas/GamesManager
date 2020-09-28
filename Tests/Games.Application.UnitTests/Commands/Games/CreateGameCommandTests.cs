using System.Threading;
using System.Threading.Tasks;
using Games.Application.Commands.Games;
using Games.Domain.Entities.GameAggregate;
using Moq;
using NUnit.Framework;

namespace Games.Application.UnitTests.Commands.Games
{
    [TestFixture()]
    public class CreateGameCommandTests
    {
        private Mock<IGameRepository> _gameRepositoryMock;

        private CreateGameCommandHandler GetHandler()
        {
            return new CreateGameCommandHandler(_gameRepositoryMock.Object);
        }

        private void GenerateMock()
        {
            _gameRepositoryMock = new Mock<IGameRepository>();
        }

        [Test()]
        public async Task ShouldCreateANewGame()
        {
            GenerateMock();

            _gameRepositoryMock.Setup(x => x.Add(It.IsAny<Game>()));

            _gameRepositoryMock.Setup(x => x.UnitOfWork.SaveAsync(It.IsAny<CancellationToken>()));

            var handler = GetHandler();

            var command = new CreateGameCommand("name", 2010,"platform");

            await handler.Handle(command, default);

            _gameRepositoryMock.Verify(x => x.Add(It.Is<Game>(f=>  f.Platform == "platform" && f.Name == "name" && f.LaunchYear == 2010)));

            _gameRepositoryMock.Verify(x => x.UnitOfWork.SaveAsync(It.IsAny<CancellationToken>()));
        }
    }
}