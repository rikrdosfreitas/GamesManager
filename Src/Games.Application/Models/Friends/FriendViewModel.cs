using System;
using Games.Application.Mappings;
using Games.Domain.Entities.FriendAggregate;

namespace Games.Application.Models.Friends
{
    public class FriendViewModel : IMapFrom<Friend>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Nickname { get; set; }

        public string Email { get; set; }

        public byte[] Ver { get; set; }
    }
}