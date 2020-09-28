using AutoMapper;
using Games.Common.Exceptions;
using Games.Domain.Entities.FriendAggregate;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Games.Application.Models.Friends;
using Newtonsoft.Json;

namespace Games.Application.Commands.Friends
{
    public class GetFriendCommand : IRequest<FriendViewModel>
    {
        [JsonConstructor]
        private GetFriendCommand(/* Tests*/) { }

        public GetFriendCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }

    public class GetFriendCommandHandler : IRequestHandler<GetFriendCommand, FriendViewModel>
    {
        private readonly IFriendQuery _query;
        private readonly IMapper _mapper;

        public GetFriendCommandHandler(IFriendQuery query, IMapper mapper)
        {
            _query = query;
            _mapper = mapper;
        }

        public async Task<FriendViewModel> Handle(GetFriendCommand request, CancellationToken cancellationToken)
        {
            var entity = await _query.GetAsync(request.Id, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException("Friend not found!");
            }

            var result = _mapper.Map<FriendViewModel>(entity);

            return result;
        }
    }
}