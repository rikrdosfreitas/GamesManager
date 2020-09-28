using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Games.Application.Commands.Friends;
using Games.Application.Mappings;
using Games.Application.Models;
using Games.Application.Models.Friends;
using Games.Common.Util;
using Games.Domain.Entities.FriendAggregate;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace Games.Application.UnitTests.Commands.Friends
{
    [TestFixture()]
    public class GetFriendsCommandTests
    {

        private Mock<IFriendQuery> _friendQueryMock;
        private IMapper _mapper;

        private GetFriendsCommandHandler GetCommandHandler()
        {

            return new GetFriendsCommandHandler(_friendQueryMock.Object, _mapper);
        }

        private void GenerateMock()
        {
            _friendQueryMock = new Mock<IFriendQuery>();
            _mapper = new MapperConfiguration(cfg => { cfg.AddProfile(new MappingProfile()); }).CreateMapper();
        }

        [Test()]
        public async Task ShouldReturnAllFriends()
        {
            GenerateMock();

            var entities = new List<Friend>
            {
                Friend.Create(SequentialGuid.NewGuid(),  "Bruce", "Wayne", "bruce@wayne.com"),
                Friend.Create(SequentialGuid.NewGuid(), "Bruce Banner", "Banner", "hulk@banner.com")
            };

            var expected = entities.Select(e => _mapper.Map<FriendListViewModel>(e)).ToList();

            var friendMock = entities.AsQueryable().BuildMock();


            _friendQueryMock.Setup(x => x.GetAllAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(friendMock.Object);

            _friendQueryMock.Setup(x => x.CountAsync(It.IsAny<string>(), default))
                .ReturnsAsync(1);

            var command = GetCommandHandler();

            var result = await command.Handle(new GetFriendsCommand { Search = "search", Sort = "sort", Order = "order", Page = 0, Size = 5 }, default);

            result.Should().NotBeNull()
                    .And.BeAssignableTo<ResponseViewModel<FriendListViewModel>>();

            result.Records.Should().Be(1);
            result.Data.Should().HaveCount(2)
                .And.BeEquivalentTo(expected);

        }

        [Test()]
        public async Task ShouldVerifyTheCallParameters()
        {
            GenerateMock();

            var friendMock = new List<Friend>().AsQueryable().BuildMock();

            _friendQueryMock.Setup(x => x.GetAllAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(friendMock.Object);

            _friendQueryMock.Setup(x => x.CountAsync(It.IsAny<string>(), default))
                .ReturnsAsync(0);

            var command = GetCommandHandler();

            IDictionary<string, object> filter = new Dictionary<string, object> { { "name", "search" } };

            await command.Handle(new GetFriendsCommand { Search = "search", Sort = "sort", Order = "order", Page = 0, Size = 5 }, default);

            _friendQueryMock.Verify(x => x.GetAllAsync(
                It.Is<string>(s => s.Equals("search")),
                It.Is<string>(s => s.Equals("sort")),
                It.Is<string>(s => s.Equals("order")),
                It.Is<int>(s => s.Equals(0)),
                It.Is<int>(s => s.Equals(5))));

            _friendQueryMock.Verify(c => c.CountAsync(It.Is<string>(x => x.Equals("search")), default));

        }
    }
}
