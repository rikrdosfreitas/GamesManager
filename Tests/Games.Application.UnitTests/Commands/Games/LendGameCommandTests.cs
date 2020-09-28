using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Games.Application.Commands.Games;
using Games.Common.Exceptions;
using Games.Domain.Entities.FriendAggregate;
using Games.Domain.Entities.GameAggregate;
using Moq;
using NUnit.Framework;

namespace Games.Application.UnitTests.Commands.Games
{
    [TestFixture()]
    public class LendGameCommandTests
    {
        private Mock<IGameRepository> _gameRepositoryMock;
        private Mock<IFriendQuery> _friendQuery;

        private LendGameCommandHandler GetHandler()
        {
            return new LendGameCommandHandler(_gameRepositoryMock.Object, _friendQuery.Object);
        }

        private void GenerateMock()
        {
            _gameRepositoryMock = new Mock<IGameRepository>();
            _friendQuery = new Mock<IFriendQuery>();
        }

        [Test()]
        public void ShouldThrowMessageGameNotFound()
        {
            GenerateMock();

            _gameRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));

            var handler = GetHandler();

            var command = new LendGameCommand(Guid.NewGuid(), Guid.NewGuid());

            FluentActions.Invoking(async () => await handler.Handle(command, default))
                .Should()
                .Throw<NotFoundException>().WithMessage("Game not found!");

            _gameRepositoryMock
                .Verify(x => x.FindAsync(It.Is<Guid>(f => f.Equals(command.Id)), It.IsAny<CancellationToken>()));
        }

        [Test()]
        public void ShouldThrowMessageGameInvalidFriend()
        {
            GenerateMock();

            var gameMock = new Mock<Game>();
            _gameRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(gameMock.Object);

            _friendQuery
                .Setup(x => x.IsValid(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var handler = GetHandler();

            var command = new LendGameCommand(Guid.NewGuid(), Guid.NewGuid());

            FluentActions.Invoking(async () => await handler.Handle(command, default))
                .Should()
                .Throw<GuardValidationException>().WithMessage("Invalid friend to lend!");

            _friendQuery
                .Setup(x => x.IsValid(It.Is<Guid>(f => f.Equals(command.FriendId)), It.IsAny<CancellationToken>()));
        }

        [Test()]
        public async Task ShouldLendGame()
        {
            GenerateMock();

            var gameMock = new Mock<Game>();
            gameMock.Setup(x => x.Lent(It.IsAny<Guid>()));

            _gameRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(gameMock.Object);

            _friendQuery
                .Setup(x => x.IsValid(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            _gameRepositoryMock.Setup(x => x.Update(It.IsAny<Game>()));

            _gameRepositoryMock.Setup(x => x.UnitOfWork.SaveAsync(It.IsAny<CancellationToken>()));

            var handler = GetHandler();

            var friendId = Guid.NewGuid();
            var command = new LendGameCommand(Guid.NewGuid(), friendId);

            await handler.Handle(command, default);

            gameMock.Verify(x => x.Lent(
                It.Is<Guid>(f => f.Equals(friendId))
                ));

            _gameRepositoryMock.Verify(x => x.Update(It.Is<Game>(f => f.Equals(gameMock.Object))));

            _gameRepositoryMock.Verify(x => x.UnitOfWork.SaveAsync(It.IsAny<CancellationToken>()));
        }
    }
}