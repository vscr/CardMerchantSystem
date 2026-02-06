namespace CardMerchantSystem.Shared.Audit.Enums;

/// <summary>
/// Audit işlem tipleri
/// </summary>
public enum AuditActionType
{
    /// <summary>
    /// Yeni kayıt ekleme
    /// </summary>
    Insert = 1,

    /// <summary>
    /// Kayıt güncelleme
    /// </summary>
    Update = 2,

    /// <summary>
    /// Kayıt silme
    /// </summary>
    Delete = 3,

    /// <summary>
    /// Soft delete (IsDeleted = true)
    /// </summary>
    SoftDelete = 4
}