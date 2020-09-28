using System;
using FluentAssertions;
using Games.Domain.Entities.FriendAggregate;
using NUnit.Framework;

namespace Games.Domain.Tests.Entities.FriendAggregate
{
    public class FriendTests
    {
        [Test]
        public void ShouldCreateANewFriend()
        {
            var id = Guid.NewGuid();

            Friend friend = Friend.Create(id: id, name: "Bruce Wayne", nickName: "batman", email: "bruce@wayne.com");

            friend.Should().NotBeNull();
            friend.Id.Should().Be(id);
            friend.Name.Should().Be("Bruce Wayne");
            friend.NickName.Should().Be("batman");
            friend.Email.Should().Be("bruce@wayne.com");
        }

        [Test]
        public void ShouldUpdateFriend()
        {
            var id = Guid.NewGuid();

            var ver = new byte[] {0x01};
            Friend friend = Friend.Create( id,  "Bruce","Wayne", "bruce@wayne.com");

            friend.Update(name: "Bruce Wayne",nickName: "Batman", email: "bruce@batman.com", ver: ver);

            friend.Should().NotBeNull();
            friend.Id.Should().Be(id);
            friend.Name.Should().Be("Bruce Wayne");
            friend.NickName.Should().Be("Batman");
            friend.Email.Should().Be("bruce@batman.com");
            friend.Ver.Should().BeEquivalentTo(ver);
        }
    }
}