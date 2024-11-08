using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.EventBus.Integrations;

public class IntegrationEventContext(DbContextOptions<IntegrationEventContext> options) : DbContext(options)
{
    public DbSet<IntegrationEventLog> IntegrationEventLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IntegrationEventLog>(ConfigureIntegrationEventLogEntry);
    }

    private static void ConfigureIntegrationEventLogEntry(EntityTypeBuilder<IntegrationEventLog> builder)
    {
        builder.ToTable("integration_event");

        builder.HasKey(e => e.EventId);

        builder.Property(e => e.EventId)
            .HasColumnName("event_id")
            .IsRequired();

        builder.Property(e => e.Content)
            .HasColumnName("content")
            .IsRequired();

        builder.Property(e => e.CreationTime)
            .HasColumnName("creation_time")
            .IsRequired();

        builder.Property(e => e.State)
            .HasColumnName("state")
            .IsRequired();

        builder.Property(e => e.TimesSent)
            .HasColumnName("times_sent")
            .IsRequired();

        builder.Property(e => e.EventTypeName)
            .HasColumnName("event_type_name")
            .IsRequired();

        builder.Property(e => e.TransactionId)
            .HasColumnName("transaction_id")
            .IsRequired();
    }
}