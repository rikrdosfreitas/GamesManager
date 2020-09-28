using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Games.Application.Commands.Friends;
using Games.Application.Models.Friends;
using Games.Common.Util;
using Games.Domain.Entities.FriendAggregate;
using Games.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Games.Application.IntegrationTests.Commands.Friends
{
    using static Testing;

    [TestFixture()]
    public class GetFriendCommandTests : TestBase
    {
        private Friend _entity;

        [SetUp]
        protected void SetUp()
        {
            _entity = Friend.Create(SequentialGuid.NewGuid(), "Bruce Wayne", "Batman", "batman@batman.com");

            var list = new List<Friend>
            {
                Friend.Create(SequentialGuid.NewGuid(), "Bruce Banner", "Hulk", "hulk@hulk.com"),
                Friend.Create(SequentialGuid.NewGuid(), "Tony Stark", "Iron Man", "startk@startk.com")
            };

            list.Add(_entity);

            using var scope = _scopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<GamesDbContext>();

            context.Friends.AddRange(list);

            context.SaveChanges();
        }

        [Test]
        public async Task ShouldReturnFriends()
        {
           var result = await SendAsync(new GetFriendCommand(_entity.Id));

            result.Should().NotBeNull();
            result.Id.Should().Be(_entity.Id);
            result.Name.Should().Be(_entity.Name);
            result.Nickname.Should().Be(_entity.NickName);
            result.Email.Should().Be(_entity.Email);
            result.Should().BeAssignableTo<FriendViewModel>();
        }
    }
}