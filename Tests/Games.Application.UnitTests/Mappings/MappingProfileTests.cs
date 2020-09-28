using System;
using AutoMapper;
using Games.Application.Mappings;
using Games.Application.Models.Friends;
using Games.Application.Models.Games;
using Games.Domain.Entities.FriendAggregate;
using Games.Domain.Entities.GameAggregate;
using NUnit.Framework;

namespace Games.Application.UnitTests.Mappings
{
    [TestFixture]
    public class MappingProfileTests
    {
        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;

        public MappingProfileTests()
        {
            _configuration = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });

            _mapper = _configuration.CreateMapper();
        }

        [Test]
        public void ShouldHaveValidConfiguration()
        {
            _configuration.AssertConfigurationIsValid();
        }

        [TestCase(typeof(Friend), typeof(FriendListViewModel))]
        [TestCase(typeof(Friend), typeof(FriendViewModel))]
        [TestCase(typeof(Game), typeof(GameListViewModel))]
        [TestCase(typeof(Game), typeof(GameViewModel))]
        [TestCase(typeof(Game), typeof(GamesLoanedViewModel))]
        public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
        {
            var instance = Activator.CreateInstance(source, true);

            _mapper.Map(instance, source, destination);
        }
    }
}