using Statement.Domain.Entities;
using Statement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Statement.Infrastructure.Configurations;

public class StatementNotificationConfiguration : IEntityTypeConfiguration<StatementNotification>
{
    public void Configure(EntityTypeBuilder<StatementNotification> builder)
    {
        builder.ToTable("StatementNotifications");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.Recipient).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Subject).HasMaxLength(200);
        builder.Property(x => x.Content).HasMaxLength(2000);
        builder.Property(x => x.ErrorMessage).HasMaxLength(500);

        // Smart Enum: NotificationType
        builder.Property(x => x.NotificationType)
            .HasConversion(
                v => v.Id,
                v => NotificationType.FromId<NotificationType>(v)!)
            .HasColumnName("NotificationTypeId")
            .IsRequired();

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.StatementId);
        builder.HasIndex(x => x.IsSent);
    }
}