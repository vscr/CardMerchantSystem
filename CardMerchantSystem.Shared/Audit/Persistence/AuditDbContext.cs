using CardMerchantSystem.Shared.Audit.Entities;
using CardMerchantSystem.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace CardMerchantSystem.Shared.Audit.Persistence;

/// <summary>
/// Audit logları için minimal DbContext.
/// Sadece migration oluşturmak için kullanılır.
/// Runtime'da Dapper ile doğrudan SQL yazılır.
/// </summary>
public class AuditDbContext : DbContext
{
    public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options)
    {
    }

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("audit");

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLogs");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedNever();

            entity.Property(x => x.UserId)
                .HasMaxLength(100);

            entity.Property(x => x.UserName)
                .HasMaxLength(256);

            entity.Property(x => x.EntityType)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(x => x.EntityName)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(x => x.EntityId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.TableName)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(x => x.ActionType)
                .IsRequired();

            entity.Property(x => x.OldValues)
                .HasColumnType("nvarchar(max)");

            entity.Property(x => x.NewValues)
                .HasColumnType("nvarchar(max)");

            entity.Property(x => x.ChangedColumns)
                .HasMaxLength(2000);

            entity.Property(x => x.IpAddress)
                .HasMaxLength(50);

            entity.Property(x => x.CorrelationId)
                .HasMaxLength(100);

            entity.Property(x => x.AdditionalData)
                .HasColumnType("nvarchar(max)");

            entity.Property(x => x.Timestamp)
                .IsRequired();

            // Index'ler
            entity.HasIndex(x => x.Timestamp)
                .HasDatabaseName("IX_AuditLogs_Timestamp");

            entity.HasIndex(x => new { x.EntityName, x.EntityId })
                .HasDatabaseName("IX_AuditLogs_Entity");

            entity.HasIndex(x => x.UserId)
                .HasDatabaseName("IX_AuditLogs_UserId");

            entity.HasIndex(x => x.ActionType)
                .HasDatabaseName("IX_AuditLogs_ActionType");

            entity.HasIndex(x => x.CorrelationId)
                .HasDatabaseName("IX_AuditLogs_CorrelationId");

            entity.HasIndex(x => new { x.EntityName, x.Timestamp })
                .HasDatabaseName("IX_AuditLogs_EntityName_Timestamp");
        });
    }
}