using System;
using System.Threading;
using System.Threading.Tasks;
using Games.Application.Behaviors;
using Games.Application.Commands.Friends;
using Games.Application.Commands.Games;
using Games.Application.Models;
using Games.Application.Models.Friends;
using Games.Application.Models.Games;
using Games.Common.Transaction.Utilities;
using Games.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Games.Application.UnitTests.Behaviors
{
    [TestFixture(TypeArgs = new[] { typeof(CreateFriendCommand), typeof(bool) })]
    [TestFixture(TypeArgs = new[] { typeof(UpdateFriendCommand), typeof(bool) })]
    [TestFixture(TypeArgs = new[] { typeof(DeleteFriendCommand), typeof(bool) })]
    [TestFixture(TypeArgs = new[] { typeof(GetFriendCommand), typeof(FriendViewModel) })]
    [TestFixture(TypeArgs = new[] { typeof(GetFriendsCommand), typeof(ResponseViewModel<FriendListViewModel>) })]
    [TestFixture(TypeArgs = new[] { typeof(CreateGameCommand), typeof(bool) })]
    [TestFixture(TypeArgs = new[] { typeof(UpdateGameCommand), typeof(bool) })]
    [TestFixture(TypeArgs = new[] { typeof(DeleteGameCommand), typeof(bool) })]
    [TestFixture(TypeArgs = new[] { typeof(LendGameCommand), typeof(bool) })]
    [TestFixture(TypeArgs = new[] { typeof(ReturnGameCommand), typeof(bool) })]
    [TestFixture(TypeArgs = new[] { typeof(GetGameCommand), typeof(GameViewModel) })]
    [TestFixture(TypeArgs = new[] { typeof(GetGameLoanedInfoCommand), typeof(GamesLoanedViewModel) })]
    [TestFixture(TypeArgs = new[] { typeof(GetGamesCommand), typeof(ResponseViewModel<GameListViewModel>) })]
    public class TransactionBehaviorTests<T, TR> where T : IRequest<TR>
    {
        private readonly Mock<IDbContextManager> _dbContextManagerMock;
        private readonly Mock<GamesDbContext> _contextMock;

        public TransactionBehaviorTests()
        {
            _dbContextManagerMock = new Mock<IDbContextManager>();
            _contextMock = new Mock<GamesDbContext>();
        }

        [Test()]
        public async Task ShouldCallExecuteInTransaction()
        {
            var transaction = new TransactionBehavior<T, TR>(_dbContextManagerMock.Object, _contextMock.Object);

            var mockPipelineBehaviourDelegate = new Mock<RequestHandlerDelegate<TR>>();

            var instance = (T)Activator.CreateInstance(typeof(T), true);
            await transaction.Handle(instance, new CancellationToken(), mockPipelineBehaviourDelegate.Object);

            _dbContextManagerMock.Verify(x =>
                    x.ExecuteInTransactionAsync(It.IsAny<DbContext>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test()]
        public async Task ShouldNotCallExecuteInTransaction()
        {
            var transaction = new TransactionBehavior<T, TR>(_dbContextManagerMock.Object, _contextMock.Object);
            _dbContextManagerMock.Setup(x => x.IsInTransaction).Returns(true);

            var mockPipelineBehaviourDelegate = new Mock<RequestHandlerDelegate<TR>>();
            mockPipelineBehaviourDelegate.Setup(x => x.Invoke());

            var instance = (T)Activator.CreateInstance(typeof(T), true);
            await transaction.Handle(instance, new CancellationToken(), mockPipelineBehaviourDelegate.Object);

            _dbContextManagerMock.Verify(x => x.ExecuteInTransactionAsync(It.IsAny<DbContext>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);

            mockPipelineBehaviourDelegate.Verify(x => x.Invoke(), Times.Once);
        }
    }
}