using System;
using System.ComponentModel.DataAnnotations;

namespace Games.Domain.Entities.Common
{
    public abstract class AggregateRoot : IAggregateRoot
    {
        protected AggregateRoot() { }

        protected AggregateRoot(Guid id)
        {
            Id = id;
            Ver = new byte[1];
        }

        public Guid Id { get; private set; }

        [Timestamp]
        public byte[] Ver { get; protected set; }

        protected virtual void Update(byte[] ver)
        {
            Buffer.BlockCopy(ver, 0, Ver, 0, ver.Length);
        }
    }
}