using System;
using System.Collections.Generic;
using System.Linq;
using Games.Common.Transaction;
using Games.Common.Util;
using Games.Domain.Entities.FriendAggregate;
using Games.Domain.Entities.GameAggregate;
using Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Games.Api
{
    public static class MigrationManager
    {
        public static IHost MigrateDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                using (var appContext = scope.ServiceProvider.GetRequiredService<GamesDbContext>())
                {
                    try
                    {
                        appContext.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        //Log errors or do anything you think it's needed
                        throw;
                    }
                }

                using (var appContext = scope.ServiceProvider.GetRequiredService<TransactionContext>())
                {
                    try
                    {
                        appContext.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        //Log errors or do anything you think it's needed
                        throw;
                    }
                }
            }

            return host;
        }


        public static IHost SeedData(this IHost host)
        {

            Random rd = new Random(20);
            List<Friend> friends = GetFriends();
            List<Game> games = GetGames();

            games.ForEach(x =>
            {
                var next = rd.Next(0, friends.Count-1);
                if (next % 2 == 0) return;

                x.Lent(friends[next].Id);
            });

            using (var scope = host.Services.CreateScope())
            {
                using (var appContext = scope.ServiceProvider.GetRequiredService<GamesDbContext>())
                {
                    try
                    {
                        SeedFriends(appContext, friends);
                        SeedGames(appContext, games);
                    }
                    catch (Exception ex)
                    {
                        //Log errors or do anything you think it's needed
                        throw;
                    }
                }

                return host;
            }
        }

        private static void SeedFriends(GamesDbContext context, List<Friend> friends)
        {
            if (!context.Friends.Any())
            {
                context.AddRange(friends);
                context.SaveChanges();
            }
        }

        private static List<Friend> GetFriends()
        {
            return new List<Friend>
                {
                     Friend.Create(SequentialGuid.NewGuid(), "Bruce Wayne", "Batman","batman@batman.com"),
                     Friend.Create(SequentialGuid.NewGuid(), "Bruce Banner","Hulk", "hulk@hulk.com"),
                     Friend.Create(SequentialGuid.NewGuid(), "Tony Stark", "Iron Man", "startk@startk.com"),
                     Friend.Create(SequentialGuid.NewGuid(), "Natasha Romanoff", "Black Widow","black@widow.com"),
                     Friend.Create(SequentialGuid.NewGuid(), "Clint Barton","Hawkeye","hawkeye@barton.com"),
                     Friend.Create(SequentialGuid.NewGuid(), "James Rhodes","War Machine","war@machine.com"),
                     Friend.Create(SequentialGuid.NewGuid(), "Scott Lang","Ant Man","ant@man.com"),
                     Friend.Create(SequentialGuid.NewGuid(), "Carol Danvers","Captain Marvel","captain@marvel.com")
            };
        }

        private static void SeedGames(GamesDbContext context, List<Game> games)
        {
            if (!context.Games.Any()){
                

                context.AddRange(games);
                context.SaveChanges();
            }
        }

        private static List<Game> GetGames()
        {
            return new List<Game>
                {
                    Game.Create(SequentialGuid.NewGuid(),"Call of Duty: Black Ops Cold War" ,2020, "X-Box"),
                    Game.Create(SequentialGuid.NewGuid(),"Immortals Fenyx Rising" ,2020, "PS 5"),
                    Game.Create(SequentialGuid.NewGuid(),"Ratchet & Clank: Rift Apart" ,2015, "PC"),
                    Game.Create(SequentialGuid.NewGuid(),"Bridge Constructor: The Walking Dead" ,2020, "PS 2"),
                    Game.Create(SequentialGuid.NewGuid(),"Bugsnax" ,2020, "PS 4"),
                    Game.Create(SequentialGuid.NewGuid(),"Watch Dogs: Legion" ,2020, "PS 5"),
                    Game.Create(SequentialGuid.NewGuid(),"Assassin's Creed Valhalla" ,2020, "PC")

                };
        }
    }
}
