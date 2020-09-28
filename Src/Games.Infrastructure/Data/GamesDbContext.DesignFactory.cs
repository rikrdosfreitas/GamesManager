using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Games.Infrastructure.Data
{
    public partial class ERPCloudDbContextDesignFactory : IDesignTimeDbContextFactory<GamesDbContext>
    {
        public const string DEFAULT_CONNECTION_STRING = @"Data Source=.\SQLEXPRESS;Initial Catalog=localhost_test;User Id=sa;Password=s@;MultipleActiveResultSets=True;";


        public GamesDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<GamesDbContext>()
                .UseSqlServer(DEFAULT_CONNECTION_STRING, opt =>
                {
                    opt.MigrationsAssembly("Games.Api");
                    opt.EnableRetryOnFailure();
                })
                .Options;

            return new GamesDbContext(options);
        }
    }
}