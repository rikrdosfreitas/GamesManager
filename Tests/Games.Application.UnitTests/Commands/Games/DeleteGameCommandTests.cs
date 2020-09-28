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
    public class DeleteGameCommandTests
    {
        private Mock<IGameRepository> _gameRepositoryMock;

        private DeleteGameCommandHandler GetHandler()
        {
            return new DeleteGameCommandHandler(_gameRepositoryMock.Object);
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

            var command = new DeleteGameCommand(Guid.NewGuid());

            FluentActions.Invoking(async () => await handler.Handle(command, default))
                .Should()
                .Throw<NotFoundException>().WithMessage("Game not found!");

            _gameRepositoryMock
                .Verify(x => x.FindAsync(It.Is<Guid>(f => f.Equals(command.Id)), It.IsAny<CancellationToken>()));
        }

        [Test()]
        public async Task ShouldDeleteGame()
        {
            GenerateMock();

            var gameMock = new Mock<Game>();

            _gameRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(gameMock.Object);

            _gameRepositoryMock
                .Setup(x => x.Delete(It.IsAny<Game>()));

            _gameRepositoryMock
                .Setup(x => x.UnitOfWork.SaveAsync(It.IsAny<CancellationToken>()));

            var handler = GetHandler();

            var command = new DeleteGameCommand(Guid.NewGuid());

            await handler.Handle(command, default);

            _gameRepositoryMock
                .Verify(x => x.FindAsync(It.Is<Guid>(f => f.Equals(command.Id)), It.IsAny<CancellationToken>()));

            _gameRepositoryMock
                .Verify(x => x.Delete(It.Is<Game>(f => f.Equals(gameMock.Object))));

            _gameRepositoryMock.Verify(x => x.UnitOfWork.SaveAsync(It.IsAny<CancellationToken>()));
        }
    }
}