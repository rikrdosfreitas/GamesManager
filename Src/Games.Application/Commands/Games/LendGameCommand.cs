using Games.Common.Exceptions;
using Games.Domain.Entities.FriendAggregate;
using Games.Domain.Entities.GameAggregate;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Games.Application.Commands.Games
{
    public class LendGameCommand : IRequest<bool>
    {
        [JsonConstructor]
        public LendGameCommand() { /* Tests */ }

        public LendGameCommand(Guid id, Guid friendId)
        {
            Id = id;
            FriendId = friendId;
        }

        public Guid Id { get; set; }

        public Guid FriendId { get; set; }
    }

    public class LendGameCommandHandler : IRequestHandler<LendGameCommand, bool>
    {
        private readonly IGameRepository _repository;
        private readonly IFriendQuery _friendQuery;

        public LendGameCommandHandler(IGameRepository repository, IFriendQuery friendQuery)
        {
            _repository = repository;
            _friendQuery = friendQuery;
        }

        public async Task<bool> Handle(LendGameCommand request, CancellationToken cancellationToken)
        {
            var game = await _repository.FindAsync(request.Id, cancellationToken);

            if (game == null)
            {
                throw new NotFoundException("Game not found!");
            }

            if (!await _friendQuery.IsValid(request.FriendId, cancellationToken))
            {
                throw new GuardValidationException("Invalid friend to lend!");
            }

            game.Lent(request.FriendId);

            _repository.Update(game);

            return await _repository.UnitOfWork.SaveAsync(cancellationToken);
        }
    }
}
