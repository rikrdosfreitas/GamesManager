﻿using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Games.Common.Exceptions;
using Games.Domain.Entities.FriendAggregate;

namespace Games.Application.Commands.Friends
{
    public class UpdateFriendCommand : FriendCommand, IRequest<bool>
    {
        public byte[] Ver { get; set; }
    }

    public class UpdateFriendCommandHandler : IRequestHandler<UpdateFriendCommand, bool>
    {
        private readonly IFriendRepository _repository;

        public UpdateFriendCommandHandler(IFriendRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateFriendCommand request, CancellationToken cancellationToken)
        {

            var entity = await _repository.FindAsync(request.Id, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException("Friend not found!");
            }

            entity.Update(request.Name, request.Nickname, request.Email, request.Ver);

            _repository.Update(entity);

            await _repository.UnitOfWork.SaveAsync(cancellationToken);

            return true;
        }
    }
}
