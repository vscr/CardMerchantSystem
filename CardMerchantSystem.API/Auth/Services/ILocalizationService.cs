using CardMerchantSystem.API.Auth.Models;

namespace CardMerchantSystem.API.Auth.Services;

public interface ILocalizationService
{
    // Languages
    Task<List<LanguageDto>> GetAllLanguagesAsync(CancellationToken cancellationToken = default);
    Task<LanguageDto?> GetLanguageByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<LanguageDto?> CreateLanguageAsync(CreateLanguageDto dto, CancellationToken cancellationToken = default);
    Task<bool> SetDefaultLanguageAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> ToggleLanguageStatusAsync(string code, CancellationToken cancellationToken = default);

    // Translations
    Task<Dictionary<string, string>> GetTranslationsAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Dictionary<string, string>> GetTranslationsByCategoryAsync(string languageCode, string category, CancellationToken cancellationToken = default);
    Task<List<TranslationDto>> GetAllTranslationsAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<TranslationDto?> CreateTranslationAsync(CreateTranslationDto dto, CancellationToken cancellationToken = default);
    Task<TranslationDto?> UpdateTranslationAsync(Guid id, UpdateTranslationDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteTranslationAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> BulkUpsertTranslationsAsync(BulkTranslationDto dto, CancellationToken cancellationToken = default);
}