using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Games.Common.Util;
using Games.Domain.Entities.GameAggregate;
using Games.Infrastructure.Data;
using Games.Infrastructure.Queries;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace Games.Infrastructure.Tests.Queries
{
    [TestFixture()]
    public class GameQueryTests
    {
        private Mock<GamesDbContext> _contextMock;

        private GameQuery GetRepository()
        {
            return new GameQuery(_contextMock.Object);
        }

        private void GenerateMock()
        {
            _contextMock = new Mock<GamesDbContext>();
        }

        [Test()]
        public async Task ShouldntReturnAnyGame()
        {
            GenerateMock();

            List<Game> list = new List<Game>
            {
                Game.Create(SequentialGuid.NewGuid(),"Call of Duty: Black Ops Cold War" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Immortals Fenyx Rising" ,2020, "PS 5")
            };

            var mockGame = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Set<Game>()).Returns(mockGame.Object);

            var query = GetRepository();

            var model = await query.GetAsync(Guid.Empty, default);

            model.Should().BeNull();
        }


        [Test()]
        public async Task ShouldReturnTheGameRequested()
        {
            GenerateMock();

            var game = Game.Create(SequentialGuid.NewGuid(), "Watch Dogs: Legion", 2020, "PS 5");
            List<Game> list = new List<Game>
            {
                Game.Create(SequentialGuid.NewGuid(),"Call of Duty: Black Ops Cold War" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Immortals Fenyx Rising" ,2020, "PS 5")
            };

            list.Add(game);

            var mockGame = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Set<Game>()).Returns(mockGame.Object);

            var query = GetRepository();

            var model = await query.GetAsync(game.Id, default);

            model.Should().NotBeNull();
            model.Id.Should().Be(game.Id);
            model.Name.Should().Be(game.Name);
        }

        [Test()]
        public async Task ShouldReturnTheGameRequestedWithLoanedInformation()
        {
            GenerateMock();

            var game = Game.Create(SequentialGuid.NewGuid(), "Watch Dogs: Legion", 2020, "PS 5");

            List<Game> list = new List<Game>
            {
                Game.Create(SequentialGuid.NewGuid(),"Call of Duty: Black Ops Cold War" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Immortals Fenyx Rising" ,2020, "PS 5")
            };

            list.Add(game);

            var mockGame = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Set<Game>()).Returns(mockGame.Object);

            var query = GetRepository();

            var model = await query.GetLoanedInfo(game.Id, default);

            model.Should().NotBeNull();
            model.Id.Should().Be(game.Id);
            model.Name.Should().Be(game.Name);
            model.Platform.Should().Be(game.Platform);
        }

        [Test]
        public async Task ShouldReturnAllGames()
        {
            GenerateMock();

            List<Game> list = new List<Game>
            {
                Game.Create(SequentialGuid.NewGuid(),"Call of Duty: Black Ops Cold War" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Immortals Fenyx Rising" ,2020, "PS 5")
            };

            var mockGame = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Set<Game>()).Returns(mockGame.Object);

            var repository = GetRepository();

            var query = repository.GetAllAsync(string.Empty, "name", "asc", 0, 10);

            var result = await query.ToListAsync();

            result.Should().HaveCount(2)
                .And.BeEquivalentTo(list);
        }

        [Test]
        public async Task ShouldReturnCountGames()
        {
            GenerateMock();

            List<Game> list = new List<Game>
            {
                Game.Create(SequentialGuid.NewGuid(),"Call of Duty: Black Ops Cold War" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Immortals Fenyx Rising" ,2020, "PS 5")

            };

            var mockGame = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Set<Game>()).Returns(mockGame.Object);

            var repository = GetRepository();

            var result = await repository.CountAsync(string.Empty, default);

            result.Should().Be(2);
        }


        [Test]
        public async Task ShouldReturnTheFirstFiveGamesOrderedByName()
        {
            GenerateMock();


            List<Game> list = new List<Game>
            {
                Game.Create(SequentialGuid.NewGuid(),"Call of Duty: Black Ops Cold War" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Immortals Fenyx Rising" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Ratchet & Clank: Rift Apart" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Bridge Constructor: The Walking Dead" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Bugsnax" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Watch Dogs: Legion" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Assassin's Creed Valhalla" ,2020, "PS 5")
            };

            var mockGame = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Set<Game>()).Returns(mockGame.Object);

            var repository = GetRepository();

            var query = repository.GetAllAsync(string.Empty, "name", "asc", 0, 5);

            var result = await query.ToListAsync();

            result.Should().HaveCount(5)
                .And.BeEquivalentTo(list.OrderBy(x => x.Name).Take(5).ToList());
        }


        [Test]
        public async Task ShouldReturnTheLastGamesSkipFiveOrderedByName()
        {
            GenerateMock();

            List<Game> list = new List<Game>
            {
                Game.Create(SequentialGuid.NewGuid(),"Call of Duty: Black Ops Cold War" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Immortals Fenyx Rising" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Ratchet & Clank: Rift Apart" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Bridge Constructor: The Walking Dead" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Bugsnax" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Watch Dogs: Legion" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Assassin's Creed Valhalla" ,2020, "PS 5")
            };

            var mockGame = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Set<Game>()).Returns(mockGame.Object);

            var repository = GetRepository();

            var query = repository.GetAllAsync(string.Empty, "name", "asc", 1, 5);

            var result = await query.ToListAsync();

            result.Should().HaveCount(2)
                .And.BeEquivalentTo(list.OrderBy(x => x.Name).Skip(5).Take(5).ToList())
                .And.BeInAscendingOrder(x => x.Name);
        }

        [Test]
        public async Task ShouldReturnTheGamesOrderedDescByName()
        {
            GenerateMock();

            List<Game> list = new List<Game>
            {
                Game.Create(SequentialGuid.NewGuid(),"Call of Duty: Black Ops Cold War" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Immortals Fenyx Rising" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Ratchet & Clank: Rift Apart" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Bridge Constructor: The Walking Dead" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Bugsnax" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Watch Dogs: Legion" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Assassin's Creed Valhalla" ,2020, "PS 5")
            };

            var mockGame = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Set<Game>()).Returns(mockGame.Object);

            var repository = GetRepository();

            var query = repository.GetAllAsync(string.Empty, "name", "desc", 0, 10);

            var result = await query.ToListAsync();

            result.Should().HaveCount(7)
                .And.BeEquivalentTo(list)
                .And.BeInDescendingOrder(x => x.Name);
        }

        [Test]
        public async Task ShouldReturnOnlyGameWithNameContainsName()
        {
            GenerateMock();

            List<Game> list = new List<Game>
            {
                Game.Create(SequentialGuid.NewGuid(),"Call of Duty: Black Ops Cold War" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Immortals Fenyx Rising" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Ratchet & Clank: Rift Apart" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Bridge Constructor: The Walking Dead" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Bugsnax" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Watch Dogs: Legion" ,2020, "PS 5"),
                Game.Create(SequentialGuid.NewGuid(),"Assassin's Creed Valhalla" ,2020, "PS 5")
            };

            var mockGame = list.AsQueryable().BuildMockDbSet();

            _contextMock.Setup(x => x.Set<Game>()).Returns(mockGame.Object);

            var repository = GetRepository();

            var query = repository.GetAllAsync("Call", "name", "desc", 0, 10);

            var result = await query.ToListAsync();

            result.Should().HaveCount(1)
                .And.BeEquivalentTo(list.Where(x => x.Name.Contains("Call")));
        }
    }
}