using CardMerchantSystem.Shared.Kernel;

namespace Fraud.Domain.Entities;

/// <summary>
/// Fraud kara listesi. PayGuard'daki UserDefinedList karşılığı.
/// Kart, üye işyeri, IP, ülke vb. bazında kara/beyaz liste.
/// Rule engine'de "In" operatörüyle kullanılır.
/// </summary>
public class FraudBlacklist : Entity
{
    /// <summary>Liste tipi: Card, Merchant, Country, MCC, IP, BIN</summary>
    public string ListType { get; private set; } = null!;

    /// <summary>Liste değeri: kart numarası, merchant ID, ülke kodu vb.</summary>
    public string Value { get; private set; } = null!;

    /// <summary>Kara liste (true) veya beyaz liste (false)</summary>
    public bool IsBlacklist { get; private set; }

    public string? Reason { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = null!;

    private FraudBlacklist() { } // EF Core

    public FraudBlacklist(
        string listType, string value, bool isBlacklist,
        string? reason, DateTime? expiresAt, string createdBy)
    {
        ListType = listType ?? throw new ArgumentNullException(nameof(listType));
        Value = value ?? throw new ArgumentNullException(nameof(value));
        IsBlacklist = isBlacklist;
        Reason = reason;
        ExpiresAt = expiresAt;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>Süre dolmuş mu veya deaktif mi?</summary>
    public bool IsEffective(DateTime now) =>
        IsActive && (ExpiresAt == null || now <= ExpiresAt);

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}