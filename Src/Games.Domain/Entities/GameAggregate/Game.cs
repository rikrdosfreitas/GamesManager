using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Games.Common.Exceptions;
using Games.Domain.Entities.Common;
using Games.Domain.Entities.FriendAggregate;

namespace Games.Domain.Entities.GameAggregate
{
    public class Game : AggregateRoot
    {
        internal Game() { /* EF Tests */ }

        private Game(Guid id, string name, int launchYear, string platform) : base(id)
        {
            Name = name;
            LaunchYear = launchYear;
            Platform = platform;
            State = GameState.Available;
        }

        [StringLength(100)]
        public string Name { get; private set; }

        public int LaunchYear { get; private set; }

        public GameState State { get; private set; }

        [StringLength(100)]
        public string Platform { get; private set; }

        public virtual GameLoan Loan { get; private set; }

        public static Game Create(Guid id, string name, int launchYear, string platform)
        {
            return new Game(id, name, launchYear, platform);
        }

        public virtual void Update(string name, int launchYear, string platform, byte[] ver)
        {
            Name = name;
            LaunchYear = launchYear;
            Platform = platform;

            base.Update(ver);
        }

        public virtual void Lent(Guid friend)
        {
            if (State != GameState.Available)
            {
                throw new GuardValidationException("Game not available for lent!");
            }

            Loan = GameLoan.Lend(friend, DateTime.UtcNow.Date);
            State = GameState.Loaned;
        }

        public virtual void Return()
        {
            if (State != GameState.Loaned)
            {
                throw new GuardValidationException("Game not loaned!");
            }

            Loan = GameLoan.Return();
            State = GameState.Available;
        }
    }

    public enum GameState
    {
        Available,
        Loaned
    }

    public class GameLoan : ValueObject
    {
        internal GameLoan() { /* EF Tests */ }

        private GameLoan(Guid friendId, DateTime loanDate)
        {
            FriendId = friendId;
            LoanDate = loanDate;
        }

        public Guid FriendId { get; private set; }

        public DateTime LoanDate { get; private set; }

        public virtual Friend Friend { get; private set; }

        protected override IEnumerable<object> GetEqualityValues()
        {
            yield return FriendId;
            yield return LoanDate;
        }

        public static GameLoan Lend(Guid friendId, DateTime BorrowDate)
        {
            return new GameLoan(friendId, BorrowDate);
        }

        public static GameLoan Return()
        {
            return default;
        }
    }
}
