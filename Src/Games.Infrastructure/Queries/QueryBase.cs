using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Games.Domain.Entities.Common;
using Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Games.Infrastructure.Queries
{
    public abstract class QueryBase<T>: IQuery<T> where T : class, IAggregateRoot
    {
        private IQueryable<T> _query;

        protected QueryBase(GamesDbContext context)
        {
            _query = context.Set<T>().AsNoTracking();
            Context = context;
        }

        protected virtual IQueryable<T> Query => _query;

        protected GamesDbContext Context { get; }

        protected abstract Dictionary<string, Expression<Func<T, object>>> OrderExpressions { get; }

        protected abstract Expression<Func<T, bool>> BuildSearch(string search);

        public abstract Task<T> GetAsync(Guid id, CancellationToken cancellationToken);

        protected async Task<T> GetEntryAsync(Guid id, CancellationToken cancellationToken = default, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = default)
        {
            if (include != null)
            {
                _query = include(_query);
            }

            return await _query
                .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
        }

        public IQueryable<T> GetAllAsync(string search, string sort, string order, int page, int size)
        {
            var query = Get(search);

            if (!string.IsNullOrWhiteSpace(sort))
            {
                var exp = OrderExpressions[sort];
                if (string.IsNullOrWhiteSpace(order) || order.Equals("ASC", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.OrderBy(exp);
                }
                else if (order.Equals("DESC", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.OrderByDescending(exp);
                }
            }

            if (size > 0)
            {
                query = query.Skip(page * size).Take(size);
            }

            return query;
        }

        public async Task<int> CountAsync(string search, CancellationToken cancellationToken)
        {
            return await Get(search).CountAsync(cancellationToken);
        }

        private IQueryable<T> Get(string search)
        {
            var query = Query;

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(BuildSearch(search));
            }

            return query;
        }
    }
}