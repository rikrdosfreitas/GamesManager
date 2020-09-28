using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Games.Application.Commands.Games;
using Games.Common.Exceptions;
using Games.Domain.Entities.GameAggregate;
using Moq;
using NUnit.Framework;

namespace Games.Application.UnitTests.Commands.Games
{
    [TestFixture()]
    public class UpdateGameCommandTests
    {
        private Mock<IGameRepository> _gameRepositoryMock;

        private UpdateGameCommandHandler GetHandler()
        {
            return new UpdateGameCommandHandler(_gameRepositoryMock.Object);
        }

        private void GenerateMock()
        {
            _gameRepositoryMock = new Mock<IGameRepository>();
        }

        [Test()]
        public void ShouldThrowMessageGameNotFound()
        {
            GenerateMock();

            _gameRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));

            var handler = GetHandler();

            var command = new UpdateGameCommand(Guid.NewGuid(), "", 0, "", null);

            FluentActions.Invoking(async () => await handler.Handle(command, default))
                .Should()
                .Throw<NotFoundException>().WithMessage("Game not found!");

            _gameRepositoryMock
                .Verify(x => x.FindAsync(It.Is<Guid>(f => f.Equals(command.Id)), It.IsAny<CancellationToken>()));
        }

        [Test()]
        public async Task ShouldUpdateGame()
        {
            GenerateMock();

            var gameMock = new Mock<Game>();
            gameMock.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<byte[]>()));

            _gameRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(gameMock.Object);

            _gameRepositoryMock.Setup(x => x.Update(It.IsAny<Game>()));

            _gameRepositoryMock.Setup(x => x.UnitOfWork.SaveAsync(It.IsAny<CancellationToken>()));

            var handler = GetHandler();

            var ver = new byte[] { 0x02 };

            var command = new UpdateGameCommand( Guid.NewGuid(), "name",  2018, "platform", ver);

            await handler.Handle(command, default);

            gameMock.Verify(x => x.Update(
                It.Is<string>(f => f.Equals("name")),
                It.Is<int>(f => f.Equals(2018)),
                It.Is<string>(f => f.Equals("platform")),
                It.Is<byte[]>(f => f.Equals(ver))
            ));

            _gameRepositoryMock.Verify(x => x.Update(It.Is<Game>(f => f.Equals(gameMock.Object))));

            _gameRepositoryMock.Verify(x => x.UnitOfWork.SaveAsync(It.IsAny<CancellationToken>()));
        }
    }
}