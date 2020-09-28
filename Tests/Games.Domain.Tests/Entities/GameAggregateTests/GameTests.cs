using System;
using FluentAssertions;
using Games.Common.Exceptions;
using Games.Domain.Entities.FriendAggregate;
using Games.Domain.Entities.GameAggregate;
using NUnit.Framework;

namespace Games.Domain.Tests.Entities.GameAggregateTests
{
    public class GameTests
    {
        [Test]
        public void ShouldCreateNewGame()
        {
            var id = Guid.NewGuid();

            Game game = Game.Create(id: id, name: "Avengers", launchYear: 2020, platform: "X-Box");

            game.Should().NotBeNull();
            game.Id.Should().Be(id);
            game.Name.Should().Be("Avengers");
            game.Platform.Should().Be("X-Box");
            game.State.Should().Be(GameState.Available);
            game.LaunchYear.Should().Be(2020);
        }

        [Test]
        public void ShouldUpdateGame()
        {
            var id = Guid.NewGuid();

            Game game = Game.Create(id: id, name: "Avengers", launchYear: 2010, platform: "X-Box");

            var ver = new byte[] { 0x20 };

            game.Update(name: "Marvel's Avengers", launchYear: 2020, "PS 5", ver: ver);

            game.Should().NotBeNull();
            game.Id.Should().Be(id);
            game.Name.Should().Be("Marvel's Avengers");
            game.Platform.Should().Be("PS 5");
            game.LaunchYear.Should().Be(2020);
            game.State.Should().Be(GameState.Available);
            game.Ver.Should().BeEquivalentTo(ver);
        }

        [Test]
        public void ShouldLoanGame()
        {
            var id = Guid.NewGuid();

            Game game = Game.Create(id: id, name: "Avengers", launchYear: 2010, "PS 5");

            Friend friend = Friend.Create(id: id, "Bruce Wayne", "Wayne", "bruce@wayne.com");

            game.Lent(friend: friend.Id);

            game.Should().NotBeNull();
            game.Id.Should().Be(id);
            game.State.Should().Be(GameState.Loaned);
            game.Loan.Should().NotBeNull();
            game.Loan.FriendId.Should().Be(friend.Id);
            game.Loan.LoanDate.Should().Be(DateTime.UtcNow.Date);
        }

        [Test]
        public void ShouldThrowIfGameAlreadyLoaned()
        {
            var id = Guid.NewGuid();

            Game game = Game.Create(id: id, name: "Avengers", launchYear: 2010, "PS 5");

            Friend friend = Friend.Create(id: id, name: "Bruce Wayne", "Wayne", "bruce@wayne.com");

            game.Lent(friend: friend.Id);

            FluentActions.Invoking(() => game.Lent(friend.Id)).Should()
                .Throw<GuardValidationException>().WithMessage("Game not available for lent!");

        }

        [Test]
        public void ShouldReturnGame()
        {
            var id = Guid.NewGuid();

            Game game = Game.Create(id: id, name: "Avengers", launchYear: 2010, "PS 5");

            Friend friend = Friend.Create(id: id, name: "Bruce Wayne", "Wayne", "bruce@wayne.com");

            game.Lent(friend: friend.Id);

            game.Return();

            game.Should().NotBeNull();
            game.Id.Should().Be(id);
            game.State.Should().Be(GameState.Available);
            game.Loan.Should().BeNull();
        }

        [Test]
        public void ShouldThrowIfGameNotLoaned()
        {
            var id = Guid.NewGuid();

            Game game = Game.Create(id: id, name: "Avengers", launchYear: 2010, "PS 5");


            FluentActions.Invoking(() => game.Return()).Should()
                .Throw<GuardValidationException>().WithMessage("Game not loaned!");

        }
    }
}