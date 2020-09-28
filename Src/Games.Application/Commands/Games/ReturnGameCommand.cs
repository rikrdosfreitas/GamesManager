using MediatR;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using Games.Common.Exceptions;
using Games.Domain.Entities.GameAggregate;

namespace Games.Application.Commands.Games
{
    public class ReturnGameCommand : IRequest<bool>
    {
        [JsonConstructor]
        private ReturnGameCommand() {/* Tests*/ }

        public ReturnGameCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }

    public class ReturnGameCommandHandler : IRequestHandler<ReturnGameCommand, bool>
    {
        private readonly IGameRepository _repository;

        public ReturnGameCommandHandler(IGameRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(ReturnGameCommand request, CancellationToken cancellationToken)
        {
            var game = await _repository.FindAsync(request.Id, cancellationToken);

            if (game == null)
            {
                throw new NotFoundException("Game not found!");
            }

            game.Return();

            _repository.Update(game);

            return await _repository.UnitOfWork.SaveAsync(cancellationToken);
        }
    }
}
