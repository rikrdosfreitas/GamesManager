using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Games.Application.Commands.Games;
using Games.Application.Mappings;
using Games.Common.Exceptions;
using Games.Common.Util;
using Games.Domain.Entities.GameAggregate;
using Moq;
using NUnit.Framework;

namespace Games.Application.UnitTests.Commands.Games
{
    [TestFixture()]
    public class GetGameCommandTests
    {
        private Mock<IGameQuery> _gameQueryMock;

        private GetGameCommandHandler GetCommandHandler()
        {
            return new GetGameCommandHandler(_gameQueryMock.Object, new MapperConfiguration(cfg => { cfg.AddProfile(new MappingProfile()); }).CreateMapper());
        }

        private void GenerateMock()
        {
            _gameQueryMock = new Mock<IGameQuery>();
        }

        [Test()]
        public void ShouldThrowMessageFriendNotFound()
        {
            GenerateMock();

            _gameQueryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));

            var handler = GetCommandHandler();

            var command = new GetGameCommand(Guid.NewGuid());

            FluentActions.Invoking(async () => await handler.Handle(command, default))
                .Should()
                .Throw<NotFoundException>().WithMessage("Game not found!");

            _gameQueryMock
                .Verify(x => x.GetAsync(It.Is<Guid>(f => f.Equals(command.Id)), It.IsAny<CancellationToken>()));
        }

        [Test()]
        public async Task ShouldReturnRequestedGame()
        {
            GenerateMock();

            Game game = Game.Create(SequentialGuid.NewGuid(), "Spider Man",2020, "X-Box");

            _gameQueryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(game));

            var command = GetCommandHandler();

            var result = await command.Handle(new GetGameCommand(game.Id), default);

            result.Should().NotBeNull();
            result.Id.Should().Be(game.Id);
            result.Name.Should().Be(game.Name);
            result.LaunchYear.Should().Be(game.LaunchYear);
            result.Platform.Should().Be(game.Platform);
            result.Ver.Should().NotBeNull();
        }
    }
}
