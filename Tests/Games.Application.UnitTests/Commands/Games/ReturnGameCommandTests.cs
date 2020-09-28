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
    public class ReturnGameCommandTests
    {
        private Mock<IGameRepository> _gameRepositoryMock;

        private ReturnGameCommandHandler GetHandler()
        {
            return new ReturnGameCommandHandler(_gameRepositoryMock.Object);
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

            var command = new ReturnGameCommand(Guid.NewGuid());

            FluentActions.Invoking(async () => await handler.Handle(command, default))
                .Should()
                .Throw<NotFoundException>().WithMessage("Game not found!");

            _gameRepositoryMock
                .Verify(x => x.FindAsync(It.Is<Guid>(f => f.Equals(command.Id)), It.IsAny<CancellationToken>()));
        }


        [Test()]
        public async Task ShouldReturnGame()
        {
            GenerateMock();

            var gameMock = new Mock<Game>();
            gameMock.Setup(x => x.Return());

            _gameRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(gameMock.Object);

         _gameRepositoryMock.Setup(x => x.Update(It.IsAny<Game>()));

            _gameRepositoryMock.Setup(x => x.UnitOfWork.SaveAsync(It.IsAny<CancellationToken>()));

            var handler = GetHandler();

            var command = new ReturnGameCommand(Guid.NewGuid());

            await handler.Handle(command, default);

            gameMock.Verify(x => x.Return());

            _gameRepositoryMock.Verify(x => x.Update(It.Is<Game>(f => f.Equals(gameMock.Object))));

            _gameRepositoryMock.Verify(x => x.UnitOfWork.SaveAsync(It.IsAny<CancellationToken>()));
        }
    }
}