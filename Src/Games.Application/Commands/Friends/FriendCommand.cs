using System;

namespace Games.Application.Commands.Friends
{
    public abstract class FriendCommand
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        
        public string Nickname { get; set; }

        public string Email { get; set; }

    }
}