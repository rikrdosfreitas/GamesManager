using System;

namespace Games.Application.Commands.Friends
{
    public abstract class FriendCommand
    {
        protected FriendCommand() { }

        protected FriendCommand(Guid id, string name, string nickname, string email)
        {
            Id = id;
            Name = name;
            Nickname = nickname;
            Email = email;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Nickname { get; set; }

        public string Email { get; set; }

    }
}