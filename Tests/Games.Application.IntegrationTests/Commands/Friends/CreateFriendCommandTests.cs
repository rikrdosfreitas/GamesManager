using System.Threading.Tasks;
using FluentAssertions;
using Games.Application.Commands.Friends;
using Games.Domain.Entities.FriendAggregate;
using NUnit.Framework;

namespace Games.Application.IntegrationTests.Commands.Friends
{
    using static Testing;

    [TestFixture()]
    public class CreateFriendCommandTests : TestBase
    {
        [Test]
        public async Task ShouldCreateFriend()
        {
            var cmd = new CreateFriendCommand("Unit Test", "test", "test@test.com");

            await SendAsync(cmd);

            var entity = await FindAsync<IFriendRepository, Friend>(cmd.Id);
            entity.Should().NotBeNull();
            entity.Id.Should().Be(cmd.Id);
            entity.Name.Should().Be("Unit Test");
            entity.NickName.Should().Be("test");
            entity.Email.Should().Be("test@test.com");
        }
    }
}
