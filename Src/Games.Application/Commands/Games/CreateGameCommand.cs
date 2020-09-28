using System.Threading;
using System.Threading.Tasks;
using Games.Common.Util;
using Games.Domain.Entities.GameAggregate;
using MediatR;
using Newtonsoft.Json;

namespace Games.Application.Commands.Games
{
    public class CreateGameCommand : GameCommand, IRequest<bool>
    {
        [JsonConstructor]
        private CreateGameCommand()
        {
            Id = SequentialGuid.NewGuid();
        }

        public CreateGameCommand(string name, int launchYear, string platform)
        {
            Id = SequentialGuid.NewGuid();
            Name = name;
            LaunchYear = launchYear;
            Platform = platform;

        }


    }

    public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, bool>
    {
        private readonly IGameRepository _repository;

        public CreateGameCommandHandler(IGameRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(CreateGameCommand request, CancellationToken cancellationToken)
        {
            var entity = Game.Create(request.Id, request.Name, request.LaunchYear, request.Platform);

            _repository.Add(entity);

            await _repository.UnitOfWork.SaveAsync(cancellationToken);

            return true;
        }
    }
}
