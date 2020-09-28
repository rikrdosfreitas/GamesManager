using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Games.Application.Commands.Friends;
using Games.Application.Mappings;
using Games.Common.Exceptions;
using Games.Common.Util;
using Games.Domain.Entities.FriendAggregate;
using Moq;
using NUnit.Framework;

namespace Games.Application.UnitTests.Commands.Friends
{
    [TestFixture()]
    public class GetFriendCommandTests
    {

        private Mock<IFriendQuery> _friendQueryMock;

        private GetFriendCommandHandler GetCommandHandler()
        {
            return new GetFriendCommandHandler(_friendQueryMock.Object, new MapperConfiguration(cfg => { cfg.AddProfile(new MappingProfile()); }).CreateMapper());
        }

        private void GenerateMock()
        {
            _friendQueryMock = new Mock<IFriendQuery>();
        }


        [Test()]
        public void ShouldThrowMessageFriendNotFound()
        {
            GenerateMock();
            
            _friendQueryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));

            var handler = GetCommandHandler();

            var command = new GetFriendCommand(Guid.NewGuid());

            FluentActions.Invoking(async () => await handler.Handle(command, default))
                .Should()
                .Throw<NotFoundException>().WithMessage("Friend not found!");

            _friendQueryMock
                .Verify(x => x.GetAsync(It.Is<Guid>(f => f.Equals(command.Id)), It.IsAny<CancellationToken>()));
        }

        [Test()]
        public async Task ShouldReturnRequestedFriend()
        {
            GenerateMock();

            Friend friend = Friend.Create(SequentialGuid.NewGuid(),  "Bruce", "Wayne", "bruce@wayne.com");

            _friendQueryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(friend));

            var command = GetCommandHandler();

            var result = await command.Handle(new GetFriendCommand(friend.Id), default);

            result.Should().NotBeNull();
            result.Id.Should().Be(friend.Id);
            result.Name.Should().Be(friend.Name);
            result.Email.Should().Be(friend.Email);
            result.Nickname.Should().Be(friend.NickName);
            result.Ver.Should().NotBeNull();
        }
    }
}
