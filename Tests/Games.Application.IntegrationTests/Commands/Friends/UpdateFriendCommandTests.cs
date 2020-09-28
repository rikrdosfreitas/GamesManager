using System.Threading.Tasks;
using FluentAssertions;
using Games.Application.Commands.Friends;
using Games.Common.Util;
using Games.Domain.Entities.FriendAggregate;
using Games.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Games.Application.IntegrationTests.Commands.Friends
{
    using static Testing;

    [TestFixture()]
    public class UpdateFriendCommandTests
    {
        private Friend _entity;

        [SetUp]
        protected void SetUp()
        {
            _entity = Friend.Create(SequentialGuid.NewGuid(), "Bruce", "Batman", "batman@batman.com");

          
            using var scope = _scopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<GamesDbContext>();

            context.Friends.Add(_entity);

            context.SaveChanges();
        }

        [Test]
        public async Task ShouldUpdateFriend()
        {
            var cmd = new UpdateFriendCommand(_entity.Id, "Bruce Wayne", "Wayne", "batman@batman.com.br", _entity.Ver);

            await SendAsync(cmd);

            var entity = await FindAsync<IFriendRepository, Friend>(_entity.Id);

            entity.Should().NotBeNull();
            entity.Id.Should().Be(_entity.Id);
            entity.Name.Should().Be("Bruce Wayne");
            entity.NickName.Should().Be("Wayne");
            entity.Email.Should().Be("batman@batman.com.br");
        }
    }
}