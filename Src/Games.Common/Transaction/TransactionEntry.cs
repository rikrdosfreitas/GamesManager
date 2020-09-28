using System;

namespace Games.Common.Transaction
{
    public sealed class TransactionEntry
    {
        private TransactionEntry() { /*EF Constructor*/ }

        public TransactionEntry(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }
}
