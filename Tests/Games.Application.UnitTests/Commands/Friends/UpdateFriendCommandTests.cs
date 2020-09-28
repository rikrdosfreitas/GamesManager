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
    public class UpdateFriendCommandTests
    {
        private Mock<IFriendRepository> _friendRepositoryMock;

        private UpdateFriendCommandHandler GetHandler()
        {
            return new UpdateFriendCommandHandler(_friendRepositoryMock.Object);
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

            var command = new UpdateFriendCommand(Guid.NewGuid(), "teste", "unit", "test@test", null);

            FluentActions.Invoking(async () => await handler.Handle(command, default))
                .Should()
                .Throw<NotFoundException>().WithMessage("Friend not found!");

            _friendRepositoryMock
                .Verify(x => x.FindAsync(It.Is<Guid>(f => f.Equals(command.Id)), It.IsAny<CancellationToken>()));
        }

        [Test()]
        public async Task ShouldUpdateFriend()
        {
            GenerateMock();

            var friendMock = new Mock<Friend>();
            friendMock.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>()));

            _friendRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(friendMock.Object);

            _friendRepositoryMock.Setup(x => x.Update(It.IsAny<Friend>()));

            _friendRepositoryMock.Setup(x => x.UnitOfWork.SaveAsync(It.IsAny<CancellationToken>()));

            var handler = GetHandler();

            var ver = new byte[] { 0x02 };

            var command = new UpdateFriendCommand(Guid.NewGuid(), "name", "nickname", "email", ver);

            await handler.Handle(command, default);

            friendMock.Verify(x => x.Update(
                It.Is<string>(f => f.Equals("name")),
                It.Is<string>(f => f.Equals("nickname")),
                It.Is<string>(f => f.Equals("email")),
                It.Is<byte[]>(f => f.Equals(ver))
            ));

            _friendRepositoryMock.Verify(x => x.Update(It.Is<Friend>(f => f.Equals(friendMock.Object))));

            _friendRepositoryMock.Verify(x => x.UnitOfWork.SaveAsync(It.IsAny<CancellationToken>()));
        }
    }
}