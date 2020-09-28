using System;
using System.Threading;
using System.Threading.Tasks;
using Games.Common.Exceptions;
using Games.Domain.Entities.GameAggregate;
using MediatR;
using Newtonsoft.Json;

namespace Games.Application.Commands.Games
{
    public class DeleteGameCommand : IRequest<bool>
    {
        [JsonConstructor]
        private DeleteGameCommand(/* Tests*/) { }

        public DeleteGameCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }

    public class DeleteGameCommandHandler : IRequestHandler<DeleteGameCommand, bool>
    {
        private readonly IGameRepository _repository;

        public DeleteGameCommandHandler(IGameRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteGameCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.FindAsync(request.Id, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException("Game not found!");
            }

            _repository.Delete(entity);

            await _repository.UnitOfWork.SaveAsync(cancellationToken);

            return true;
        }
    }
}