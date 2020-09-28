using System;
using Games.Common.Util;
using Games.Domain.Entities.FriendAggregate;
using Games.Infrastructure.Data;
using Games.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MockQueryable.Moq;

namespace Games.Infrastructure.Tests.Repositories
{
    [TestFixture()]
    public class FriendRepositoryTests
    {
        private Mock<GamesDbContext> _contextMock;

        private FriendRepository GetRepository()
        {
            return new FriendRepository(_contextMock.Object);
        }

        private void GenerateMock()
        {
            _contextMock = new Mock<GamesDbContext>();
        }

        [Test()]
        public async Task ShouldntReturnAnyFriend()
        {
            GenerateMock();

            List<Friend> list = new List<Friend> { Friend.Create(SequentialGuid.NewGuid(), "Bruce Wayne", "batman", "batman@batman.com"), Friend.Create(SequentialGuid.NewGuid(), "Tony Stark", "man", "man@startk.com") };

            var mockEntity = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Friends).Returns(mockEntity.Object);

            var query = GetRepository();

            var model = await query.FindAsync(Guid.Empty, default);

            model.Should().BeNull();
        }

        [Test]
        public async Task ShouldReturnSpecifiedEntity()
        {
            GenerateMock();

            var expected = Friend.Create(SequentialGuid.NewGuid(), "Peter Parker", "spider", "spider@spider.com");

            List<Friend> list = new List<Friend> { Friend.Create(SequentialGuid.NewGuid(), "Bruce Wayne", "batman", "batman@batman.com"), expected, Friend.Create(SequentialGuid.NewGuid(), "Tony Stark", "man", "man@startk.com") };

            var mockEntity = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Friends).Returns(mockEntity.Object);

            var query = GetRepository();

            var model = await query.FindAsync(expected.Id, default);

            model.Should().NotBeNull();
            model.Id.Should().Be(expected.Id);
            model.Name.Should().Be(expected.Name);
        }

        [Test()]
        public void ShouldAddANewFriend()
        {
            GenerateMock();

            var friendMock = new Mock<DbSet<Friend>>();

            _contextMock.Setup(x => x.Friends).Returns(friendMock.Object);

            var friend = Friend.Create(SequentialGuid.NewGuid(), "Bruce Wayne","batman", "batman@batman.com");

            var repository = GetRepository();

            repository.Add(friend);

            friendMock.Verify(e => e.Add(It.Is<Friend>(f => f.Equals(friend))));
        }

        [Test]
        public void ShouldUpdateSpecifiedFriend()
        {
            GenerateMock();

            _contextMock.Setup(x => x.SetModified(It.IsAny<object>()));

            var expected = Friend.Create(SequentialGuid.NewGuid(), "Peter Parker","spider", "spider@spider.com");

            var repository = GetRepository();

            repository.Update(expected);

            _contextMock.Verify(x => x.SetModified(It.Is<object>(f => f.Equals(expected))));
        }

        [Test]
        public void ShouldDeleteSpecifiedFriend()
        {
            GenerateMock();

            var expected = Friend.Create(SequentialGuid.NewGuid(), "Peter Parker","spider","spider@spider.com");

            var friendMock = new Mock<DbSet<Friend>>();

            _contextMock.Setup(x => x.Friends).Returns(friendMock.Object);

            var repository = GetRepository();

            repository.Delete(expected);

            friendMock.Verify(x => x.Remove(It.Is<Friend>(f => f.Equals(expected))));

        }

        [Test]
        public void ShouldReturnUnitOfWork()
        {
            GenerateMock();

            var repository = GetRepository();

            repository.UnitOfWork.Should().BeEquivalentTo(_contextMock.Object);

        }
    }
}