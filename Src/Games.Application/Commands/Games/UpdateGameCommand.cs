using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Games.Common.Exceptions;
using Games.Domain.Entities.GameAggregate;
using Newtonsoft.Json;

namespace Games.Application.Commands.Games
{
    public class UpdateGameCommand : GameCommand, IRequest<bool>
    {
        [JsonConstructor]
        private UpdateGameCommand() { }

        public UpdateGameCommand(Guid id, string name, int launchYear, string platform, byte[] ver) : base(id, name, launchYear, platform)
        {
            Ver = ver;
        }

        public byte[] Ver { get; set; }
    }

    public class UpdateGameCommandHandler : IRequestHandler<UpdateGameCommand, bool>
    {
        private readonly IGameRepository _repository;

        public UpdateGameCommandHandler(IGameRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateGameCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.FindAsync(request.Id, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException("Game not found!");
            }

            entity.Update(request.Name, request.LaunchYear, request.Platform, request.Ver);

            _repository.Update(entity);

            await _repository.UnitOfWork.SaveAsync(cancellationToken);

            return true;
        }
    }
}
