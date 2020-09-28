using System;
using System.Threading;
using System.Threading.Tasks;
using Games.Common.Exceptions;
using Games.Domain.Entities.FriendAggregate;
using MediatR;
using Newtonsoft.Json;

namespace Games.Application.Commands.Friends
{
    public class DeleteFriendCommand : IRequest<bool>
    {
        [JsonConstructor] 
        private DeleteFriendCommand(/* Tests*/) { }

        public DeleteFriendCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }

    public class DeleteFriendCommandHandler : IRequestHandler<DeleteFriendCommand, bool>
    {
        private readonly IFriendRepository _repository;

        public DeleteFriendCommandHandler(IFriendRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteFriendCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.FindAsync(request.Id, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException("Friend not found!");
            }

            _repository.Delete(entity);

            await _repository.UnitOfWork.SaveAsync(cancellationToken);

            return true;
        }
    }
}