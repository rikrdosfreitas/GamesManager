using System;
using AutoMapper;
using Games.Application.Mappings;
using Games.Domain.Entities.GameAggregate;

namespace Games.Application.Models.Games
{
    public class GamesLoanedViewModel: IMapFrom<Game>
    {
        public Guid Id { get; set; }

        public string Game { get; set; }

        public string Platform { get; set; }

        public string FriendName { get; set; }

        public DateTime LoanDate { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Game, GamesLoanedViewModel>()
                .ForMember(opt => opt.Game, opt => opt.MapFrom(s => s.Name))
                .ForMember(opt => opt.FriendName, opt => opt.MapFrom(s => s.Loan.Friend.Name))
                .ForMember(opt => opt.LoanDate, opt => opt.MapFrom(s => s.Loan.LoanDate));

        }

      
    }
}