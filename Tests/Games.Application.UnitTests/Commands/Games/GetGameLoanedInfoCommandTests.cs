using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Games.Application.Commands.Games;
using Games.Application.Mappings;
using Games.Common.Exceptions;
using Games.Domain.Entities.FriendAggregate;
using Games.Domain.Entities.GameAggregate;
using Moq;
using NUnit.Framework;

namespace Games.Application.UnitTests.Commands.Games
{
    [TestFixture()]
    public class GetGameLoanedInfoCommandTests
    {

        private Mock<IGameQuery> _gameQueryMock;
        private IMapper _mapper;

        private GetGameLoanedInfoCommandHandler GetCommandHandler()
        {

            return new GetGameLoanedInfoCommandHandler(_gameQueryMock.Object, _mapper);
        }

        private void GenerateMock()
        {
            _gameQueryMock = new Mock<IGameQuery>();
            _mapper = new MapperConfiguration(cfg => { cfg.AddProfile(new MappingProfile()); }).CreateMapper();
        }

        [Test()]
        public void ShouldThrowMessageFriendNotFound()
        {
            GenerateMock();

            _gameQueryMock.Setup(x => x.GetLoanedInfo(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));

            var handler = GetCommandHandler();

            var command = new GetGameLoanedInfoCommand(Guid.NewGuid());

            FluentActions.Invoking(async () => await handler.Handle(command, default))
                .Should()
                .Throw<NotFoundException>().WithMessage("Game not found!");

            _gameQueryMock
                .Verify(x => x.GetLoanedInfo(It.Is<Guid>(f => f.Equals(command.Id)), It.IsAny<CancellationToken>()));
        }

        [Test()]
        public async Task ShouldReturnRequestedGame()
        {
            GenerateMock();

            var game = new Mock<Game>();
            var loan = new Mock<GameLoan>();
            var friend = new Mock<Friend>();
            game.Setup(x => x.Loan).Returns(loan.Object);
            loan.Setup(x => x.Friend).Returns(friend.Object);


            _gameQueryMock.Setup(x => x.GetLoanedInfo(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(game.Object);

            var command = GetCommandHandler();

            var result = await command.Handle(new GetGameLoanedInfoCommand(Guid.NewGuid()), default);

            result.Should().NotBeNull();
            result.Id.Should().Be(game.Object.Id);
            result.Game.Should().Be(game.Object.Name);
            result.LoanDate.Should().Be(loan.Object.LoanDate);
            result.Platform.Should().Be(game.Object.Platform);
            result.FriendName.Should().Be(friend.Object.Name);
        }
    }
}
