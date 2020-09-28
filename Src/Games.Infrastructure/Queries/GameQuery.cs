using Games.Domain.Entities.GameAggregate;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Games.Infrastructure.Queries
{
    public class GameQuery : QueryBase<Game>, IGameQuery
    {
        public GameQuery(GamesDbContext context) : base(context)
        {
        }

        protected override Dictionary<string, Expression<Func<Game, object>>> OrderExpressions =>
            new Dictionary<string, Expression<Func<Game, object>>>
            {
                { "name", o=> o.Name},
                { "launch", o=> o.LaunchYear},
                { "platform", o=> o.Platform},
                { "state", o=> o.State}
            };

        protected override Expression<Func<Game, bool>> BuildSearch(string search)
        {
            return x => x.Name.Contains(search);
        }

        public override Task<Game> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return GetEntryAsync(id, cancellationToken);
        }

        public Task<Game> GetLoanedInfo(Guid id, CancellationToken cancellationToken)
        {
            return GetEntryAsync(id, cancellationToken, i => i.Include(x => x.Loan).ThenInclude(x => x.Friend));
        }
    }
}
