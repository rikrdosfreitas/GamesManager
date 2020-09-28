using System;

namespace Games.Application.Commands.Games
{
    public abstract class GameCommand
    {
        protected GameCommand()
        {
        }

        protected GameCommand(Guid id, string name, int launchYear, string platform)
        {
            Id = id;
            Name = name;
            LaunchYear = launchYear;
            Platform = platform;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public int LaunchYear { get; set; }

        public string Platform { get; set; }    
    }
}