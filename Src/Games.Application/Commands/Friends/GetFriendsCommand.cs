using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Games.Application.Models;
using Games.Application.Models.Friends;
using Games.Domain.Entities.FriendAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Games.Application.Commands.Friends
{
    public class GetFriendsCommand : QueryCommand, IRequest<ResponseViewModel<FriendListViewModel>>
    {
        
    }

    public class GetFriendsCommandHandler : IRequestHandler<GetFriendsCommand, ResponseViewModel<FriendListViewModel>>
    {
        private readonly IFriendQuery _query;
        private readonly IMapper _mapper;

        public GetFriendsCommandHandler(IFriendQuery query, IMapper mapper)
        {
            _query = query;
            _mapper = mapper;
        }

        public async Task<ResponseViewModel<FriendListViewModel>> Handle(GetFriendsCommand request, CancellationToken cancellationToken)
        {
            var query = _query.GetAllAsync(request.Search, request.Sort, request.Order, request.Page, request.Size);

            var result = await _mapper.ProjectTo<FriendListViewModel>(query).ToListAsync(cancellationToken: cancellationToken);

            var count = await _query.CountAsync(request.Search, cancellationToken);

            return new ResponseViewModel<FriendListViewModel>(count, result);
        }
    }
}
