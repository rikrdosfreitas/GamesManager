using System;
using Games.Application.Mappings;
using Games.Domain.Entities.GameAggregate;

namespace Games.Application.Models.Games
{
    public class GameViewModel : IMapFrom<Game>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int LaunchYear { get; set; }

        public string Platform { get; set; }

        public byte[] Ver { get; set; }
    }
}