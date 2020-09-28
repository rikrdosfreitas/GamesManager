using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Games.Application.Models.Games;
using Games.Common.Exceptions;
using Games.Domain.Entities.GameAggregate;
using MediatR;
using Newtonsoft.Json;

namespace Games.Application.Commands.Games
{
    public class GetGameLoanedInfoCommand : IRequest<GamesLoanedViewModel>
    {
        [JsonConstructor]
        private GetGameLoanedInfoCommand() {/* Tests */ }

        public GetGameLoanedInfoCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }

   public class GetGameLoanedInfoCommandHandler : IRequestHandler<GetGameLoanedInfoCommand, GamesLoanedViewModel>
    {
        private readonly IGameQuery _query;
        private readonly IMapper _mapper;

        public GetGameLoanedInfoCommandHandler(IGameQuery query, IMapper mapper)
        {
            _query = query;
            _mapper = mapper;
        }

        public async Task<GamesLoanedViewModel> Handle(GetGameLoanedInfoCommand request, CancellationToken cancellationToken)
        { 
            Game game = await _query.GetLoanedInfo(request.Id, cancellationToken);

            if (game == null)
            {
                throw new NotFoundException("Game not found!");
            }

            return _mapper.Map<GamesLoanedViewModel>(game);
        }
    }
}
