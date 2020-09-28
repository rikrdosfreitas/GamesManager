using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Games.Common.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Games.Common.Transaction.Utilities
{
    public class DbContextManager : IDbContextManager, IDisposable
    {
        private readonly ContextSettings _contextSettings;
        private TransactionContext _db;
        private DbContextSet _contexts;

        public DbContextManager(ContextSettings contextSettings)
        {
            _contextSettings = contextSettings;
            _contexts = new DbContextSet();
        }

        public bool IsInTransaction => _db?.Database.CurrentTransaction != null;

        public async Task ExecuteInTransactionAsync(DbContext dbContext, Func<CancellationToken, Task> operation, CancellationToken token = default)
        {
            _contexts.ValidateContext(dbContext);

            /* Connection Resilience Strategy: "Manually track the transaction"                                 *
            * reference: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency          */

            var connection = dbContext.Database.GetDbConnection();
            using (_db = CreateTransactionContext(connection))
            {
                var strategy = _db.Database.CreateExecutionStrategy();

                var transaction = new TransactionEntry(SequentialGuid.NewGuid());
                _db.Transactions.Add(transaction);

                await operation(token);

                await strategy.ExecuteInTransactionAsync(
                    operation: async () =>
                        await _contexts.SaveChangesAsync(token),
                    verifySucceeded: async () =>
                        await _db.Transactions.AsNoTracking()
                            .AnyAsync(t => t.Id == transaction.Id, cancellationToken: token));

                _contexts.AcceptAllChanges();

                _db.Transactions.Remove(transaction);
                await _db.SaveChangesAsync(token);
            }

            _db = null;
        }

        public IDbContextManager RegisterContext(DbContext context)
        {
            _contexts.RegisterContext(context);
            return this;
        }

        private TransactionContext CreateTransactionContext(DbConnection connection)
        {
            var options = new DbContextOptionsBuilder<TransactionContext>()
                .UseSqlServer(connection, opt =>
                {
                    opt.MigrationsHistoryTable("__InfraHistoryTable", _contextSettings.Schema);
                    opt.MigrationsAssembly(_contextSettings.MigrationAssembly);
                    opt.EnableRetryOnFailure();
                })
                .Options;

            var db = new TransactionContext(options, _contextSettings.Schema);

            _contexts.RegisterContext(db);

            return db;
        }


        protected class DbContextSet : IDisposable
        {
            private readonly List<DbContext> _contexts = new List<DbContext>();

            public async Task SaveChangesAsync(CancellationToken token)
            {
                var transaction = _contexts
                    .Single(c => c.Database.CurrentTransaction != null)
                    .Database.CurrentTransaction.GetDbTransaction();

                foreach (var context in _contexts)
                {
                    if (context.Database.CurrentTransaction == null)
                    {
                        context.Database.UseTransaction(transaction);
                    }

                    await context.SaveChangesAsync(acceptAllChangesOnSuccess: false, cancellationToken: token);
                }
            }

            public void AcceptAllChanges()
            {
                foreach (var context in _contexts)
                {
                    context.ChangeTracker.AcceptAllChanges();
                }
            }

            public void RegisterContext(DbContext context)
            {
                if (!_contexts.Contains(context))
                {
                    _contexts.Add(context);
                }
            }

            public void ValidateContext(DbContext context)
            {
                if (!_contexts.Contains(context))
                {
                    throw new InvalidOperationException("Database Context not found: " + context.GetType().Name);
                }
            }

            #region IDisposable
            public void Dispose()
            {
                foreach (var context in _contexts)
                {
                    context.Dispose();
                }

                _contexts.Clear();
            }
            #endregion
        }

        #region Dispose
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _contexts.Dispose();
                    _contexts = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class ContextSettings
    {
        public ContextSettings(string schema, string migrationAssembly)
        {
            Schema = schema;
            MigrationAssembly = migrationAssembly;
        }

        public string Schema { get; private set; }

        public string MigrationAssembly { get; private set; }
    }
}