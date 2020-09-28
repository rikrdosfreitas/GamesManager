using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Games.Common.Util;
using Games.Domain.Entities.GameAggregate;
using Games.Infrastructure.Data;
using Games.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace Games.Infrastructure.Tests.Repositories
{
    [TestFixture()]
    public class GameRepositoryTests
    {
        private Mock<GamesDbContext> _contextMock;

        private GameRepository GetRepository()
        {
            return new GameRepository(_contextMock.Object);
        }

        private void GenerateMock()
        {
            _contextMock = new Mock<GamesDbContext>();
        }

        [Test()]
        public async Task ShouldntReturnAnyGame()
        {
            GenerateMock();

            List<Game> list = new List<Game> { Game.Create(SequentialGuid.NewGuid(), "Fallout 4", 2014, "X-Box"), Game.Create(SequentialGuid.NewGuid(), "Call of Duty: WWII", 2017, "X-Box") };

            var mockGame = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Games).Returns(mockGame.Object);

            var query = GetRepository();

            var model = await query.FindAsync(Guid.Empty, default);

            model.Should().BeNull();
        }

        [Test]
        public async Task ShouldReturnSpecifiedGame()
        {
            GenerateMock();

            var expected = Game.Create(SequentialGuid.NewGuid(), "Spider Man", 2018, "X-Box");

            List<Game> list = new List<Game> { Game.Create(SequentialGuid.NewGuid(), "Fallout 4", 2014, "Ps 2"), expected, Game.Create(SequentialGuid.NewGuid(), "Call of Duty: WWII", 2017,"X-Box") };

            var mockGame = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Games).Returns(mockGame.Object);

            var query = GetRepository();

            var model = await query.FindAsync(expected.Id, default);

            model.Should().NotBeNull();
            model.Id.Should().Be(expected.Id);
            model.Name.Should().Be(expected.Name);
        }

        [Test()]
        public void ShouldAddANewGame()
        {
            GenerateMock();

            var gameMock = new Mock<DbSet<Game>>();

            _contextMock.Setup(x => x.Games).Returns(gameMock.Object);

            var game = Game.Create(SequentialGuid.NewGuid(), "Fallout 4", 2014,"X-Box");

            var repository = GetRepository();

            repository.Add(game);

            gameMock.Verify(e => e.Add(It.Is<Game>(f => f.Equals(game))));
        }

        [Test]
        public void ShouldUpdateSpecifiedGame()
        {
            GenerateMock();

            _contextMock.Setup(x => x.SetModified(It.IsAny<object>()));

            var expected = Game.Create(SequentialGuid.NewGuid(), "Spider Man", 2018, "PS 5");

            var repository = GetRepository();

            repository.Update(expected);

            _contextMock.Verify(x => x.SetModified(It.Is<object>(f => f.Equals(expected))));
        }

        [Test]
        public void ShouldDeleteSpecifiedGame()
        {
            GenerateMock();

            var expected = Game.Create(SequentialGuid.NewGuid(), "Spider Man", 2018, "PS 5");

            var gameMock = new Mock<DbSet<Game>>();

            _contextMock.Setup(x => x.Games).Returns(gameMock.Object);

            var repository = GetRepository();

            repository.Delete(expected);

            gameMock.Verify(x => x.Remove(It.Is<Game>(f => f.Equals(expected))));

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