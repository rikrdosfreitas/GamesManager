using System;

namespace Games.Domain.Entities.Common
{
    public interface IAggregateRoot
    {
        Guid Id { get; }

        byte[] Ver { get; }
    }
}