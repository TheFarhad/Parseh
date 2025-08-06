namespace Framework;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class EventSourcingEFCoreCommandStoreContext<TCommandStoreContext> : CommandDbStore<TCommandStoreContext>
    where TCommandStoreContext : EventSourcingEFCoreCommandStoreContext<TCommandStoreContext>
{
    public DbSet<OutboxEvent> OutboxEvents => Set<OutboxEvent>();

    protected EventSourcingEFCoreCommandStoreContext() : base() { }
    public EventSourcingEFCoreCommandStoreContext(DbContextOptions<TCommandStoreContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OutboxEventConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}

public class OutboxEventConfiguration : IEntityTypeConfiguration<OutboxEvent>
{
    public void Configure(EntityTypeBuilder<OutboxEvent> builder)
    {
        builder
            .ToTable(nameof(OutboxEvent) + "s");

        builder
            .HasKey(_ => _.Id);

        builder
           .Property(_ => _.State)
           .IsRequired(true);

        builder
            .Property(_ => _.OwnerAggregate)
            .IsRequired(true)
            .HasMaxLength(100);

        builder
            .Property(_ => _.OwnerAggregateType)
            .HasMaxLength(100);

        builder
           .Property(_ => _.EventName)
           .HasMaxLength(100);

        builder
            .Property(_ => _.EventTypeName)
            .HasMaxLength(100);

        builder
            .Property(_ => _.Payload)
            .IsRequired(true)
            .IsUnicode(false)
            .HasColumnType("nvarchar(MAX)");

        builder
           .Property(_ => _.UserId)
           .IsRequired(true)
           .HasMaxLength(50);

        builder
            .Property(_ => _.TraceId)
            .HasMaxLength(100);

        builder
           .Property(_ => _.OccurredOnUtc)
           .IsRequired(true);


        builder
            .Property(_ => _.SpanId)
            .HasMaxLength(100);

        builder
           .Property(_ => _.IsProccessd)
           .IsRequired();
    }
}
