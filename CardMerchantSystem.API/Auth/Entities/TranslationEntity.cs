namespace CardMerchantSystem.API.Auth.Entities;

public class TranslationEntity
{
    public Guid Id { get; set; }
    public string LanguageCode { get; set; } = null!;  // tr, en
    public string Key { get; set; } = null!;           // menu.dashboard, role.admin
    public string Value { get; set; } = null!;         // Dashboard, Sistem Yöneticisi
    public string Category { get; set; } = null!;      // menu, role, common, validation
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}