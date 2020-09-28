using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Games.Application.Commands.Games;
using Games.Application.Mappings;
using Games.Application.Models;
using Games.Application.Models.Games;
using Games.Common.Util;
using Games.Domain.Entities.GameAggregate;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace Games.Application.UnitTests.Commands.Games
{
    [TestFixture()]
    public class GetGamesCommandTests
    {

        private Mock<IGameQuery> _gameQueryMock;
        private IMapper _mapper;

        private GetGamesCommandHandler GetCommandHandler()
        {

            return new GetGamesCommandHandler(_gameQueryMock.Object, _mapper);
        }

        private void GenerateMock()
        {
            _gameQueryMock = new Mock<IGameQuery>();
            _mapper = new MapperConfiguration(cfg => { cfg.AddProfile(new MappingProfile()); }).CreateMapper();
        }

        [Test()]
        public async Task ShouldReturnAllGames()
        {
            GenerateMock();

            var entities = new List<Game>
            {
                Game.Create(SequentialGuid.NewGuid(),"Bridge Constructor: The Walking Dead" ,2020,"PC"),
                Game.Create(SequentialGuid.NewGuid(),"Bugsnax" ,2020,"X-Box"),
            };

            var expected = entities.Select(e => _mapper.Map<GameListViewModel>(e)).ToList();

            var gameMock = entities.AsQueryable().BuildMock();


            _gameQueryMock.Setup(x => x.GetAllAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(gameMock.Object);

            _gameQueryMock.Setup(x => x.CountAsync(It.IsAny<string>(), default))
                .ReturnsAsync(1);

            var command = GetCommandHandler();

            var result = await command.Handle(new GetGamesCommand { Search = "search", Sort = "sort", Order = "order", Page = 0, Size = 5 }, default);

            result.Should().NotBeNull()
                    .And.BeAssignableTo<ResponseViewModel<GameListViewModel>>();

            result.Records.Should().Be(1);
            result.Data.Should().HaveCount(2)
                .And.BeEquivalentTo(expected);

        }

        [Test()]
        public async Task ShouldVerifyTheCallParameters()
        {
            GenerateMock();

            var gameMock = new List<Game>().AsQueryable().BuildMock();

            _gameQueryMock.Setup(x => x.GetAllAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(gameMock.Object);

            _gameQueryMock.Setup(x => x.CountAsync(It.IsAny<string>(), default))
                .ReturnsAsync(0);

            var command = GetCommandHandler();

            IDictionary<string, object> filter = new Dictionary<string, object> { { "name", "search" } };

            await command.Handle(new GetGamesCommand { Search = "search", Sort = "sort", Order = "order", Page = 0, Size = 5 }, default);

            _gameQueryMock.Verify(x => x.GetAllAsync(
                It.Is<string>(s => s.Equals("search")),
                It.Is<string>(s => s.Equals("sort")),
                It.Is<string>(s => s.Equals("order")),
                It.Is<int>(s => s.Equals(0)),
                It.Is<int>(s => s.Equals(5))));

            _gameQueryMock.Verify(c => c.CountAsync(It.Is<string>(x => x.Equals("search")), default));
        }
    }
}
