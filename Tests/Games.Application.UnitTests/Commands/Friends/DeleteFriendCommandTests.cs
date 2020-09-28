using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Games.Application.Commands.Friends;
using Games.Common.Exceptions;
using Games.Domain.Entities.FriendAggregate;
using Moq;
using NUnit.Framework;

namespace Games.Application.UnitTests.Commands.Friends
{
    [TestFixture()]
    public class DeleteFriendCommandTests
    {
        private Mock<IFriendRepository> _friendRepositoryMock;

        private DeleteFriendCommandHandler GetHandler()
        {
            return new DeleteFriendCommandHandler(_friendRepositoryMock.Object);
        }

        private void GenerateMock()
        {
            _friendRepositoryMock = new Mock<IFriendRepository>();
        }

        [Test()]
        public void ShouldThrowMessageFriendNotFound()
        {
            GenerateMock();

            _friendRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));

            var handler = GetHandler();

            var command = new DeleteFriendCommand(Guid.NewGuid());

            FluentActions.Invoking(async () => await handler.Handle(command, default))
                .Should()
                .Throw<NotFoundException>().WithMessage("Friend not found!");

            _friendRepositoryMock
                .Verify(x => x.FindAsync(It.Is<Guid>(f => f.Equals(command.Id)), It.IsAny<CancellationToken>()));
        }

        [Test()]
        public async Task ShouldDeleteFriend()
        {
            GenerateMock();

            var friendMock = new Mock<Friend>();

            _friendRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(friendMock.Object);

            _friendRepositoryMock
                .Setup(x => x.Delete(It.IsAny<Friend>()));

            _friendRepositoryMock
                .Setup(x => x.UnitOfWork.SaveAsync(It.IsAny<CancellationToken>()));

            var handler = GetHandler();

            var command = new DeleteFriendCommand(Guid.NewGuid());

            await handler.Handle(command, default);

            _friendRepositoryMock
                .Verify(x => x.FindAsync(It.Is<Guid>(f => f.Equals(command.Id)), It.IsAny<CancellationToken>()));

            _friendRepositoryMock
                .Verify(x => x.Delete(It.Is<Friend>(f => f.Equals(friendMock.Object))));

            _friendRepositoryMock.Verify(x => x.UnitOfWork.SaveAsync(It.IsAny<CancellationToken>()));
        }
    }
}