using System.Threading;
using System.Threading.Tasks;
using Games.Application.Commands.Friends;
using Games.Domain.Entities.FriendAggregate;
using Moq;
using NUnit.Framework;

namespace Games.Application.UnitTests.Commands.Friends
{
    [TestFixture()]
    public class CreateFriendCommandTests
    {
        private Mock<IFriendRepository> _friendRepositoryMock;

        private CreateFriendCommandHandler GetHandler()
        {
            return new CreateFriendCommandHandler(_friendRepositoryMock.Object);
        }

        private void GenerateMock()
        {
            _friendRepositoryMock = new Mock<IFriendRepository>();
        }

        [Test()]
        public async Task ShouldCreateANewFriend()
        {
            GenerateMock();

            _friendRepositoryMock.Setup(x => x.Add(It.IsAny<Friend>()));

            _friendRepositoryMock.Setup(x => x.UnitOfWork.SaveAsync(It.IsAny<CancellationToken>()));

            var handler = GetHandler();

            var command = new CreateFriendCommand("name");

            await handler.Handle(command, default);

            _friendRepositoryMock.Verify(x => x.Add(It.IsAny<Friend>()));

            _friendRepositoryMock.Verify(x => x.UnitOfWork.SaveAsync(It.IsAny<CancellationToken>()));
        }
    }
}