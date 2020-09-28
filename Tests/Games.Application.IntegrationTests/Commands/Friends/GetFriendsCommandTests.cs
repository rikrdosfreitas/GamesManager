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
    public class GetFriendsCommandTests : TestBase
    {
        [SetUp]
        protected void SetUp()
        {
            var list = new List<Friend>
            {
                Friend.Create(SequentialGuid.NewGuid(), "Bruce Wayne", "Batman","batman@batman.com"),
                Friend.Create(SequentialGuid.NewGuid(), "Bruce Banner","Hulk", "hulk@hulk.com"),
                Friend.Create(SequentialGuid.NewGuid(), "Tony Stark", "Iron Man", "startk@startk.com"),
                Friend.Create(SequentialGuid.NewGuid(), "Natasha Romanoff", "Black Widow","black@widow.com"),
                Friend.Create(SequentialGuid.NewGuid(), "Clint Barton","Hawkeye","hawkeye@barton.com"),
                Friend.Create(SequentialGuid.NewGuid(), "James Rhodes","War Machine","war@machine.com"),
                Friend.Create(SequentialGuid.NewGuid(), "Scott Lang","Ant Man","ant@man.com"),
                Friend.Create(SequentialGuid.NewGuid(), "Carol Danvers","Captain Marvel","captain@marvel.com")
            };

            using var scope = _scopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<GamesDbContext>();

            context.Friends.AddRange(list);

            context.SaveChanges();
        }


        [Test]
        public async Task ShouldReturnFriends()
        {
            var result = await SendAsync(new GetFriendsCommand { Search = "", Sort = "name", Order = "asc", Page = 0, Size = 5 });

            result.Records.Should().Be(8);
            result.Data.Should()
                .HaveCount(5)
                .And
                .AllBeAssignableTo<FriendListViewModel>();
        }
    }
}