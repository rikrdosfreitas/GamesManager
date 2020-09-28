using Games.Common.Transaction;
using Games.Domain.Entities.FriendAggregate;
using Games.Domain.Entities.GameAggregate;
using Games.Infrastructure.Data;
using Games.Infrastructure.Queries;
using Games.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Games.Infrastructure
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("CONNECTION_STRING");

            services.AddDbContext<GamesDbContext>(opt =>
            {
                opt.UseSqlServer(connectionString, sql =>
                {
                    sql.MigrationsAssembly("Games.Api");
                    sql.EnableRetryOnFailure();
                });
            });

            services.AddDbContext<TransactionContext>(opt =>
            {
                opt.UseSqlServer(connectionString, sql =>
                {
                    sql.MigrationsAssembly("Games.Api");
                    sql.EnableRetryOnFailure();
                });
            });

            services.AddTransient<IFriendRepository, FriendRepository>();
            services.AddTransient<IFriendQuery, FriendQuery>();
            services.AddTransient<IGameRepository, GameRepository>();
            services.AddTransient<IGameQuery, GameQuery>();



            return services;
        }
    }
}
