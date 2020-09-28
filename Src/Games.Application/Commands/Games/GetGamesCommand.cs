using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Games.Application.Models;
using Games.Application.Models.Games;
using Games.Domain.Entities.GameAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Games.Application.Commands.Games
{
    public class GetGamesCommand : QueryCommand, IRequest<ResponseViewModel<GameListViewModel>>
    {
       
    }

    public class GetGamesCommandHandler : IRequestHandler<GetGamesCommand, ResponseViewModel<GameListViewModel>>
    {
        private readonly IGameQuery _query;
        private readonly IMapper _mapper;

        public GetGamesCommandHandler(IGameQuery query, IMapper mapper)
        {
            _query = query;
            _mapper = mapper;
        }

        public async Task<ResponseViewModel<GameListViewModel>> Handle(GetGamesCommand request, CancellationToken cancellationToken)
        {
            var query = _query.GetAllAsync(request.Search, request.Sort, request.Order, request.Page, request.Size);

            var result = await _mapper.ProjectTo<GameListViewModel>(query).ToListAsync(cancellationToken: cancellationToken);

            var count = await _query.CountAsync(request.Search, cancellationToken);

            return new ResponseViewModel<GameListViewModel>(count, result);
        }
    }
}
