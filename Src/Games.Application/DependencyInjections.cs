using System.Reflection;
using AutoMapper;
using Games.Application.Behaviors;
using Games.Common.Transaction.Utilities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Games.Application
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

            services
                .AddSingleton(new ContextSettings("dbo", Assembly.GetExecutingAssembly().GetName().Name))
                .AddScoped(sp => new DbContextManager(sp.GetService<ContextSettings>()))
                .AddScoped<IDbContextManager>(sp => sp.GetRequiredService<DbContextManager>())
                .AddScoped<IDbContextRegistry>(sp => sp.GetRequiredService<DbContextManager>());

            return services;
        }
    }
}
