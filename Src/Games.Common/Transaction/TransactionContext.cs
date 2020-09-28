using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Games.Common.Transaction
{
    public sealed class TransactionContext : DbContext
    {
        private readonly string _schema;

        public TransactionContext(DbContextOptions<TransactionContext> options, string schema = "dbo") : base(options)
        {
            _schema = schema;
        }

        public DbSet<TransactionEntry> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(_schema);
            modelBuilder.Entity<TransactionEntry>(TransactionEntryConfiguration);
        }

        private void TransactionEntryConfiguration(EntityTypeBuilder<TransactionEntry> builder)
        {
            builder
                .ToTable("Transactions")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedNever();
        }
    }
}