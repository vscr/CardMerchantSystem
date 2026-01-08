using Merchant.Domain.Enums;
using Merchant.Domain.ValueObjects;
using CardMerchantSystem.Shared.Kernel;

namespace Merchant.Domain.Entities;

/// <summary>
/// Terminal Entity (Merchant'ın child entity'si)
/// </summary>
public class Terminal
{
    public Guid Id { get; private set; }
    public Guid MerchantId { get; private set; }
    public TerminalId TerminalCode { get; private set; } = null!;
    public TerminalType TerminalType { get; private set; } = null!;
    public TerminalStatus Status { get; private set; } = null!;

    // Terminal Bilgileri
    public string? SerialNumber { get; private set; }
    public string? Model { get; private set; }
    public string? Location { get; private set; }

    // Kurulum Bilgileri
    public DateTime? InstalledAt { get; private set; }
    public string? InstalledBy { get; private set; }

    // Audit
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // EF Core için
    private Terminal() { }

    public Terminal(
        Guid merchantId,
        TerminalId terminalCode,
        TerminalType terminalType,
        string? serialNumber = null,
        string? model = null,
        string? location = null)
    {
        Id = Guid.NewGuid();
        MerchantId = merchantId;
        TerminalCode = terminalCode;
        TerminalType = terminalType;
        Status = TerminalStatus.Pending;
        SerialNumber = serialNumber;
        Model = model;
        Location = location;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Terminali aktif eder
    /// </summary>
    public Result Activate(string installedBy)
    {
        if (!Status.CanBeActivated)
            return Result.Failure($"Terminal aktif edilemez. Mevcut durum: {Status.DisplayName}",
                ErrorCodes.ValidationError);

        Status = TerminalStatus.Active;
        InstalledAt = DateTime.UtcNow;
        InstalledBy = installedBy;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Terminali pasif yapar
    /// </summary>
    public Result Deactivate()
    {
        if (Status != TerminalStatus.Active)
            return Result.Failure("Sadece aktif terminaller pasif yapılabilir",
                ErrorCodes.ValidationError);

        Status = TerminalStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Terminali bakıma alır
    /// </summary>
    public Result SetMaintenance()
    {
        if (Status != TerminalStatus.Active && Status != TerminalStatus.Faulty)
            return Result.Failure("Terminal bakıma alınamaz",
                ErrorCodes.ValidationError);

        Status = TerminalStatus.Maintenance;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Terminali arızalı olarak işaretler
    /// </summary>
    public Result SetFaulty(string reason)
    {
        if (Status == TerminalStatus.Removed)
            return Result.Failure("Kaldırılmış terminal arızalı olarak işaretlenemez",
                ErrorCodes.ValidationError);

        Status = TerminalStatus.Faulty;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Terminali kaldırır
    /// </summary>
    public Result Remove()
    {
        if (Status == TerminalStatus.Removed)
            return Result.Failure("Terminal zaten kaldırılmış",
                ErrorCodes.ValidationError);

        Status = TerminalStatus.Removed;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Konum bilgisini günceller
    /// </summary>
    public void UpdateLocation(string location)
    {
        Location = location;
        UpdatedAt = DateTime.UtcNow;
    }
}