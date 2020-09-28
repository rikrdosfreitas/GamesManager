using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Games.Domain.Entities.FriendAggregate;
using Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Games.Infrastructure.Queries
{
    public class FriendQuery : QueryBase<Friend>, IFriendQuery
    {
        public FriendQuery(GamesDbContext context) : base(context)
        {
        }

        protected override Dictionary<string, Expression<Func<Friend, object>>> OrderExpressions =>
            new Dictionary<string, Expression<Func<Friend, object>>>
            {
                {"name", o => o.Name},
                {"nickname", o => o.NickName},
                {"email", o => o.Email},
                
            };

        protected override Expression<Func<Friend, bool>> BuildSearch(string search)
        {
            return x => x.Name.Contains(search);
        }

        public override Task<Friend> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return GetEntryAsync(id, cancellationToken);
        }

        public Task<bool> IsValid(Guid id, CancellationToken cancellationToken)
        {
            return Query.AnyAsync(x => x.Id == id, cancellationToken);
        }
    }
}
