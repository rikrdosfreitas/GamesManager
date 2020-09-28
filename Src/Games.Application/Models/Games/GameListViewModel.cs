using System;
using AutoMapper;
using Games.Application.Mappings;
using Games.Domain.Entities.GameAggregate;

namespace Games.Application.Models.Games
{
    public class GameListViewModel : IMapFrom<Game>, IListViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int LaunchYear { get; set; }

        public string Platform { get; set; }

        public string State { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Game, GameListViewModel>()
                .ForMember(opt => opt.State, opt => opt.MapFrom(s => s.State.ToString()));
                
        }
    }
}