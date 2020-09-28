using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Games.Common.Util;
using Games.Domain.Entities.FriendAggregate;
using Games.Infrastructure.Data;
using Games.Infrastructure.Queries;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace Games.Infrastructure.Tests.Queries
{
    [TestFixture()]
    public class FriendQueryTests
    {
        private Mock<GamesDbContext> _contextMock;

        private FriendQuery GetRepository()
        {
            return new FriendQuery(_contextMock.Object);
        }

        private void GenerateMock()
        {
            _contextMock = new Mock<GamesDbContext>();
        }

        [Test()]
        public async Task ShouldntReturnAnyFriend()
        {
            GenerateMock();

            List<Friend> list = new List<Friend>
            {
                Friend.Create(SequentialGuid.NewGuid(),  "Bruce", "Wayne", "bruce@wayne.com"),
                Friend.Create(SequentialGuid.NewGuid(), "Bruce Banner", "Banner", "hulk@banner.com")
            };

            var mockFriend = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Set<Friend>()).Returns(mockFriend.Object);

            var query = GetRepository();

            var model = await query.GetAsync(Guid.Empty, default);

            model.Should().BeNull();
        }


        [Test()]
        public async Task ShouldReturnTheFriendRequested()
        {
            GenerateMock();

            var friend = Friend.Create(SequentialGuid.NewGuid(), "Scott Lang","","");
            List<Friend> list = new List<Friend>
            {
                Friend.Create(SequentialGuid.NewGuid(),  "Bruce", "Wayne", "bruce@wayne.com"),
                Friend.Create(SequentialGuid.NewGuid(), "Bruce Banner", "Banner", "hulk@banner.com")
            };

            list.Add(friend);

            var mockFriend = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Set<Friend>()).Returns(mockFriend.Object);

            var query = GetRepository();

            var model = await query.GetAsync(friend.Id, default);

            model.Should().NotBeNull();
            model.Id.Should().Be(friend.Id);
            model.Name.Should().Be(friend.Name);
        }



        [Test]
        public async Task ShouldReturnAllFriends()
        {
            GenerateMock();

            List<Friend> list = new List<Friend>
            {
                Friend.Create(SequentialGuid.NewGuid(),  "Bruce", "Wayne", "bruce@wayne.com"),
                Friend.Create(SequentialGuid.NewGuid(), "Bruce Banner", "Banner", "hulk@banner.com")
            };

            var mockFriend = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Set<Friend>()).Returns(mockFriend.Object);

            var repository = GetRepository();

            var query = repository.GetAllAsync(string.Empty, "name", "asc", 0, 10);

            var result = await query.ToListAsync();

            result.Should().HaveCount(2)
                .And.BeEquivalentTo(list);
        }

        [Test]
        public async Task ShouldReturnCountFriends()
        {
            GenerateMock();

            List<Friend> list = new List<Friend>
            {
                Friend.Create(SequentialGuid.NewGuid(),  "Bruce", "Wayne", "bruce@wayne.com"),
                Friend.Create(SequentialGuid.NewGuid(), "Bruce Banner", "Banner", "hulk@banner.com")
            };

            var mockFriend = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Set<Friend>()).Returns(mockFriend.Object);

            var repository = GetRepository();

            var result = await repository.CountAsync(string.Empty, default);

            result.Should().Be(2);
        }


        [Test]
        public async Task ShouldReturnTheFirstFiveFriendsOrderedByName()
        {
            GenerateMock();


            List<Friend> list = new List<Friend>
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

            var mockFriend = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Set<Friend>()).Returns(mockFriend.Object);

            var repository = GetRepository();

            var query = repository.GetAllAsync(string.Empty, "name", "asc", 0, 5);

            var result = await query.ToListAsync();

            result.Should().HaveCount(5)
                .And.BeEquivalentTo(list.OrderBy(x => x.Name).Take(5).ToList());
        }


        [Test]
        public async Task ShouldReturnTheLastFriendsSkipFiveOrderedByName()
        {
            GenerateMock();

            List<Friend> list = new List<Friend>
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

            var mockFriend = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Set<Friend>()).Returns(mockFriend.Object);

            var repository = GetRepository();

            var query = repository.GetAllAsync(string.Empty, "name", "asc", 1, 5);

            var result = await query.ToListAsync();

            result.Should().HaveCount(3)
                .And.BeEquivalentTo(list.OrderBy(x => x.Name).Skip(5).Take(5).ToList())
                .And.BeInAscendingOrder(x => x.Name);
        }

        [Test]
        public async Task ShouldReturnTheFriendsOrderedDescByName()
        {
            GenerateMock();

            List<Friend> list = new List<Friend>
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

            var mockFriend = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Set<Friend>()).Returns(mockFriend.Object);

            var repository = GetRepository();

            var query = repository.GetAllAsync(string.Empty, "name", "desc", 0, 10);

            var result = await query.ToListAsync();

            result.Should().HaveCount(8)
                .And.BeEquivalentTo(list)
                .And.BeInDescendingOrder(x => x.Name);
        }

        [Test]
        public async Task ShouldReturnOnlyFriendWithNameContainsName()
        {
            GenerateMock();

            List<Friend> list = new List<Friend>
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

            var mockFriend = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Set<Friend>()).Returns(mockFriend.Object);

            var repository = GetRepository();

            var query = repository.GetAllAsync("Bruce", "name", "desc", 0, 10);

            var result = await query.ToListAsync();

            result.Should().HaveCount(2)
                .And.BeEquivalentTo(list.Where(x => x.Name.Contains("Bruce")));
        }

        [Test()]
        public async Task ShouldReturnTrueIfCategoryIsRegisteredAndActive()
        {
            GenerateMock();

            var friend = Friend.Create(SequentialGuid.NewGuid(), "Bruce", "Wayne", "bruce@wayne.com");

            List<Friend> list = new List<Friend> { friend };

            var mockCategory = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Set<Friend>()).Returns(mockCategory.Object);

            var query = GetRepository();

            var model = await query.IsValid(friend.Id, default);

            model.Should().BeTrue();
        }


        [Test()]
        public async Task ShouldReturnFalseIfCategoryNotRegistered()
        {
            GenerateMock();

            var friend = Friend.Create(SequentialGuid.NewGuid(), "Bruce", "Wayne", "bruce@wayne.com");

            List<Friend> list = new List<Friend> { friend };

            var mockCategory = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Set<Friend>()).Returns(mockCategory.Object);

            var query = GetRepository();

            var model = await query.IsValid(Guid.NewGuid(), default);

            model.Should().BeFalse();
        }
    }
}