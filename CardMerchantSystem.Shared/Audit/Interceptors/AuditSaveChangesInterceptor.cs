using CardMerchantSystem.Shared.Audit.Entities;
using CardMerchantSystem.Shared.Audit.Enums;
using CardMerchantSystem.Shared.Kernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CardMerchantSystem.Shared.Audit.Interceptors;

/// <summary>
/// EF Core SaveChanges interceptor'ı.
/// Tüm entity değişikliklerini otomatik olarak AuditLog'a kaydeder.
/// IAuditLogWriter kullanarak DbContext'ten bağımsız çalışır.
/// </summary>
public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IAuditContextAccessor _auditContextAccessor;
    private readonly IAuditLogWriter _auditLogWriter;

    public AuditSaveChangesInterceptor(
        IAuditContextAccessor auditContextAccessor,
        IAuditLogWriter auditLogWriter)
    {
        _auditContextAccessor = auditContextAccessor;
        _auditLogWriter = auditLogWriter;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            OnBeforeSaveChanges(eventData.Context);
        }
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            OnBeforeSaveChanges(eventData.Context);
        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(
        SaveChangesCompletedEventData eventData,
        int result)
    {
        if (eventData.Context is not null)
        {
            OnAfterSaveChanges(eventData.Context);
        }
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            await OnAfterSaveChangesAsync(eventData.Context, cancellationToken);
        }
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// SaveChanges öncesi audit entry'leri oluşturur
    /// </summary>
    private void OnBeforeSaveChanges(DbContext context)
    {
        context.ChangeTracker.DetectChanges();

        var auditEntries = new List<AuditEntry>();
        var userId = _auditContextAccessor.UserId;
        var userName = _auditContextAccessor.UserName;
        var ipAddress = _auditContextAccessor.IpAddress;
        var correlationId = _auditContextAccessor.CorrelationId;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            // AuditLog kendisi audit edilmemeli
            if (entry.Entity is AuditLog ||
                entry.State == EntityState.Detached ||
                entry.State == EntityState.Unchanged)
                continue;

            // Audit edilecek entity tiplerini filtrele (isteğe bağlı)
            // Sadece Entity base class'ından türeyenleri audit et
            if (!IsAuditableEntity(entry))
                continue;

            var auditEntry = new AuditEntry(entry)
            {
                UserId = userId,
                UserName = userName,
                IpAddress = ipAddress,
                CorrelationId = correlationId
            };

            switch (entry.State)
            {
                case EntityState.Added:
                    auditEntry.ActionType = AuditActionType.Insert;
                    foreach (var property in entry.Properties)
                    {
                        if (property.Metadata.IsPrimaryKey() && property.IsTemporary)
                        {
                            auditEntry.TemporaryProperties.Add(property);
                            continue;
                        }
                        auditEntry.NewValues[property.Metadata.Name] = property.CurrentValue;
                    }
                    break;

                case EntityState.Deleted:
                    auditEntry.ActionType = AuditActionType.Delete;
                    foreach (var property in entry.Properties)
                    {
                        auditEntry.OldValues[property.Metadata.Name] = property.OriginalValue;
                    }
                    break;

                case EntityState.Modified:
                    // Soft delete kontrolü
                    var isDeletedProp = entry.Properties.FirstOrDefault(p =>
                        p.Metadata.Name == "IsDeleted" || p.Metadata.Name == "Deleted");

                    if (isDeletedProp != null &&
                        isDeletedProp.IsModified &&
                        isDeletedProp.CurrentValue is true)
                    {
                        auditEntry.ActionType = AuditActionType.SoftDelete;
                    }
                    else
                    {
                        auditEntry.ActionType = AuditActionType.Update;
                    }

                    foreach (var property in entry.Properties)
                    {
                        if (property.IsModified)
                        {
                            auditEntry.ChangedColumns.Add(property.Metadata.Name);
                            auditEntry.OldValues[property.Metadata.Name] = property.OriginalValue;
                            auditEntry.NewValues[property.Metadata.Name] = property.CurrentValue;
                        }
                    }
                    break;
            }

            auditEntries.Add(auditEntry);
        }

        // Temporary property'si olmayanları hemen yaz
        var readyEntries = auditEntries.Where(e => !e.HasTemporaryProperties).ToList();
        if (readyEntries.Any())
        {
            var auditLogs = readyEntries.Select(e => e.ToAuditLog()).ToList();
            // Fire and forget - ana işlemi bloke etme
            _ = Task.Run(() => _auditLogWriter.WriteBatchAsync(auditLogs));
        }

        // Temporary property'si olanları sakla (SaveChanges sonrası işlenecek)
        if (auditEntries.Any(e => e.HasTemporaryProperties))
        {
            _auditContextAccessor.SetPendingAuditEntries(
                auditEntries.Where(e => e.HasTemporaryProperties).ToList());
        }
    }

    /// <summary>
    /// SaveChanges sonrası temporary property'li audit entry'leri işler
    /// </summary>
    private void OnAfterSaveChanges(DbContext context)
    {
        var pendingEntries = _auditContextAccessor.GetPendingAuditEntries();
        if (pendingEntries == null || !pendingEntries.Any())
            return;

        var auditLogs = new List<AuditLog>();

        foreach (var auditEntry in pendingEntries)
        {
            // Temporary property'lerin gerçek değerlerini al
            foreach (var prop in auditEntry.TemporaryProperties)
            {
                auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
            }

            auditLogs.Add(auditEntry.ToAuditLog());
        }

        _auditContextAccessor.ClearPendingAuditEntries();

        // Fire and forget
        _ = Task.Run(() => _auditLogWriter.WriteBatchAsync(auditLogs));
    }

    private async Task OnAfterSaveChangesAsync(DbContext context, CancellationToken cancellationToken)
    {
        var pendingEntries = _auditContextAccessor.GetPendingAuditEntries();
        if (pendingEntries == null || !pendingEntries.Any())
            return;

        var auditLogs = new List<AuditLog>();

        foreach (var auditEntry in pendingEntries)
        {
            foreach (var prop in auditEntry.TemporaryProperties)
            {
                auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
            }

            auditLogs.Add(auditEntry.ToAuditLog());
        }

        _auditContextAccessor.ClearPendingAuditEntries();

        // Async olarak yaz
        await _auditLogWriter.WriteBatchAsync(auditLogs, cancellationToken);
    }

    /// <summary>
    /// Entity'nin audit edilip edilmeyeceğini kontrol eder
    /// </summary>
    private static bool IsAuditableEntity(EntityEntry entry)
    {
        // Entity base class'ından türeyen tüm entity'leri audit et
        if (entry.Entity is Entity)
            return true;

        // Veya tüm entity'leri audit etmek istiyorsan:
        // return true;

        // Belirli namespace'leri hariç tutmak istiyorsan:
        var entityType = entry.Entity.GetType();
        var excludedNamespaces = new[] { "Microsoft.", "System." };
        return !excludedNamespaces.Any(ns => entityType.Namespace?.StartsWith(ns) == true);
    }
}