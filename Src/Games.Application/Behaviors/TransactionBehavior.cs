using System.Threading;
using System.Threading.Tasks;
using Games.Common.Transaction.Utilities;
using Games.Infrastructure.Data;
using MediatR;

namespace Games.Application.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IDbContextManager _contextManager;
        public readonly GamesDbContext Context;

        public TransactionBehavior(IDbContextManager contextManager, GamesDbContext context)
        {
            _contextManager = contextManager;
            Context = context;

            _contextManager.RegisterContext(Context);
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (_contextManager.IsInTransaction)
            {
                return await next();
            }

            TResponse response = default;

            await _contextManager.ExecuteInTransactionAsync(Context, async (token) =>
            {
                response = await next();

            }, cancellationToken);

            return response;
        }
    }
}
