using HSM.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace HSM.Domain.Entities;

/// <summary>
/// HSM Key Tanımı
/// </summary>
public class HSMKey : AggregateRoot
{
    public string KeyName { get; private set; } = null!;
    public HSMKeyType KeyType { get; private set; } = null!;
    public string KeyIndex { get; private set; } = null!;
    public string EncryptedKeyValue { get; private set; } = null!;
    public string? KeyCheckValue { get; private set; }
    public int KeyLength { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public string? Description { get; private set; }
    public Guid? ParentKeyId { get; private set; }
    public Guid HSMDeviceId { get; private set; }

    // EF Core için
    private HSMKey() { }

    public static Result<HSMKey> Create(
        string keyName,
        HSMKeyType keyType,
        string keyIndex,
        string encryptedKeyValue,
        string keyCheckValue,
        int keyLength,
        Guid hsmDeviceId,
        DateTime? expiryDate = null,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(keyName))
            return Result.Failure<HSMKey>("Key adı boş olamaz");

        if (string.IsNullOrWhiteSpace(encryptedKeyValue))
            return Result.Failure<HSMKey>("Şifreli key değeri boş olamaz");

        var key = new HSMKey
        {
            KeyName = keyName,
            KeyType = keyType,
            KeyIndex = keyIndex,
            EncryptedKeyValue = encryptedKeyValue,
            KeyCheckValue = keyCheckValue,
            KeyLength = keyLength,
            IsActive = true,
            ExpiryDate = expiryDate,
            Description = description,
            HSMDeviceId = hsmDeviceId
        };

        return key;
    }

    public void SetParentKey(Guid parentKeyId)
    {
        ParentKeyId = parentKeyId;
    }

    public void UpdateKeyValue(string encryptedKeyValue, string keyCheckValue)
    {
        EncryptedKeyValue = encryptedKeyValue;
        KeyCheckValue = keyCheckValue;
        MarkAsUpdated("System");
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
}