namespace CardMerchantSystem.API.Auth.Entities;

public class LanguageEntity
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;        // tr, en, de
    public string Name { get; set; } = null!;        // Türkçe, English, Deutsch
    public string NativeName { get; set; } = null!;  // Türkçe, English, Deutsch
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public ICollection<TranslationEntity> Translations { get; set; } = new List<TranslationEntity>();
}