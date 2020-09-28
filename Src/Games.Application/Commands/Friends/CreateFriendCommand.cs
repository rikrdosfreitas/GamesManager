using System.Threading;
using System.Threading.Tasks;
using Games.Common.Util;
using Games.Domain.Entities.FriendAggregate;
using MediatR;
using Newtonsoft.Json;

namespace Games.Application.Commands.Friends
{
    public class CreateFriendCommand : FriendCommand, IRequest<bool>
    {
        [JsonConstructor]
        private CreateFriendCommand()
        {
            Id = SequentialGuid.NewGuid();
        }

        public CreateFriendCommand(string name, string nickname, string email)
        {
            Id = SequentialGuid.NewGuid();
            Name = name;
            Nickname = nickname;
            Email = email;
        }
    }

    public class CreateFriendCommandHandler : IRequestHandler<CreateFriendCommand, bool>
    {
        private readonly IFriendRepository _repository;

        public CreateFriendCommandHandler(IFriendRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(CreateFriendCommand request, CancellationToken cancellationToken)
        {
            var entity = Friend.Create(request.Id, request.Name, request.Nickname, request.Email);

            _repository.Add(entity);

            await _repository.UnitOfWork.SaveAsync(cancellationToken);

            return true;
        }
    }
}
