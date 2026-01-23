namespace CardMerchantSystem.API.Auth.Models;

public class LanguageDto
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string NativeName { get; set; } = null!;
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
}

public class TranslationDto
{
    public Guid Id { get; set; }
    public string LanguageCode { get; set; } = null!;
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string[] Category { get; set; } = null!;
}

public class TranslationByCategoryDto
{
    public string[] Category { get; set; } = null!;
    public Dictionary<string, string> Translations { get; set; } = new();
}

public class CreateLanguageDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string NativeName { get; set; } = null!;
}

public class CreateTranslationDto
{
    public string LanguageCode { get; set; } = null!;
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string[] Category { get; set; } = null!;
}

public class UpdateTranslationDto
{
    public string Value { get; set; } = null!;
}

public class BulkTranslationDto
{
    public string LanguageCode { get; set; } = null!;
    public string[] Category { get; set; } = null!;
    public Dictionary<string, string> Translations { get; set; } = new();
}