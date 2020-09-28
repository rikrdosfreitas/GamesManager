using AutoMapper;
using Games.Common.Exceptions;
using Games.Domain.Entities.GameAggregate;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Games.Application.Models.Games;
using Newtonsoft.Json;

namespace Games.Application.Commands.Games
{
    public class GetGameCommand : IRequest<GameViewModel>
    {
        [JsonConstructor]
        private GetGameCommand(/* Tests*/) { }

        public GetGameCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }

    public class GetGameCommandHandler : IRequestHandler<GetGameCommand, GameViewModel>
    {
        private readonly IGameQuery _query;
        private readonly IMapper _mapper;

        public GetGameCommandHandler(IGameQuery query, IMapper mapper)
        {
            _query = query;
            _mapper = mapper;
        }

        public async Task<GameViewModel> Handle(GetGameCommand request, CancellationToken cancellationToken)
        {
            var entity = await _query.GetAsync(request.Id, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException("Game not found!");
            }

            var result = _mapper.Map<GameViewModel>(entity);

            return result;
        }
    }
}