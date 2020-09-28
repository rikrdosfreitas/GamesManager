using System;

namespace Games.Application.Commands.Games
{
    public abstract class GameCommand
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int LaunchYear { get; set; }

        public string Platform { get; set; }    
    }
}