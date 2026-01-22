using CardMerchantSystem.API.Auth.Entities;
using CardMerchantSystem.API.Auth.Models;
using CardMerchantSystem.API.Auth.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CardMerchantSystem.API.Auth.Services;

public class LocalizationService : ILocalizationService
{
    private readonly AuthDbContext _context;

    public LocalizationService(AuthDbContext context)
    {
        _context = context;
    }

    #region Languages

    public async Task<List<LanguageDto>> GetAllLanguagesAsync(CancellationToken cancellationToken = default)
    {
        var languages = await _context.Languages
            .OrderBy(l => l.Id)
            .ToListAsync(cancellationToken);

        return languages.Select(MapToDto).ToList();
    }

    public async Task<LanguageDto?> GetLanguageByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var language = await _context.Languages
            .FirstOrDefaultAsync(l => l.Code == code, cancellationToken);

        return language == null ? null : MapToDto(language);
    }

    public async Task<LanguageDto?> CreateLanguageAsync(CreateLanguageDto dto, CancellationToken cancellationToken = default)
    {
        var exists = await _context.Languages
            .AnyAsync(l => l.Code == dto.Code, cancellationToken);

        if (exists)
            return null;

        var maxId = await _context.Languages.MaxAsync(l => l.Id, cancellationToken);

        var language = new LanguageEntity
        {
            Id = maxId + 1,
            Code = dto.Code.ToLowerInvariant(),
            Name = dto.Name,
            NativeName = dto.NativeName,
            IsActive = true,
            IsDefault = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Languages.AddAsync(language, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(language);
    }

    public async Task<bool> SetDefaultLanguageAsync(string code, CancellationToken cancellationToken = default)
    {
        var language = await _context.Languages
            .FirstOrDefaultAsync(l => l.Code == code, cancellationToken);

        if (language == null)
            return false;

        // Tüm dillerin default'unu kaldır
        await _context.Languages
            .Where(l => l.IsDefault)
            .ExecuteUpdateAsync(l => l.SetProperty(x => x.IsDefault, false), cancellationToken);

        // Seçilen dili default yap
        language.IsDefault = true;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ToggleLanguageStatusAsync(string code, CancellationToken cancellationToken = default)
    {
        var language = await _context.Languages
            .FirstOrDefaultAsync(l => l.Code == code, cancellationToken);

        if (language == null)
            return false;

        // Default dil deaktif edilemez
        if (language.IsDefault && language.IsActive)
            return false;

        language.IsActive = !language.IsActive;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    #endregion

    #region Translations

    public async Task<Dictionary<string, string>> GetTranslationsAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var translations = await _context.Translations
            .Where(t => t.LanguageCode == languageCode)
            .ToListAsync(cancellationToken);

        return translations.ToDictionary(t => t.Key, t => t.Value);
    }

    public async Task<Dictionary<string, string>> GetTranslationsByCategoryAsync(string languageCode, string category, CancellationToken cancellationToken = default)
    {
        var translations = await _context.Translations
            .Where(t => t.LanguageCode == languageCode && t.Category == category)
            .ToListAsync(cancellationToken);

        return translations.ToDictionary(t => t.Key, t => t.Value);
    }

    public async Task<List<TranslationDto>> GetAllTranslationsAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var translations = await _context.Translations
            .Where(t => t.LanguageCode == languageCode)
            .OrderBy(t => t.Category)
            .ThenBy(t => t.Key)
            .ToListAsync(cancellationToken);

        return translations.Select(MapToDto).ToList();
    }

    public async Task<TranslationDto?> CreateTranslationAsync(CreateTranslationDto dto, CancellationToken cancellationToken = default)
    {
        var exists = await _context.Translations
            .AnyAsync(t => t.LanguageCode == dto.LanguageCode && t.Key == dto.Key, cancellationToken);

        if (exists)
            return null;

        var translation = new TranslationEntity
        {
            Id = Guid.NewGuid(),
            LanguageCode = dto.LanguageCode,
            Key = dto.Key,
            Value = dto.Value,
            Category = dto.Category,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Translations.AddAsync(translation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(translation);
    }

    public async Task<TranslationDto?> UpdateTranslationAsync(Guid id, UpdateTranslationDto dto, CancellationToken cancellationToken = default)
    {
        var translation = await _context.Translations
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (translation == null)
            return null;

        translation.Value = dto.Value;
        translation.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(translation);
    }

    public async Task<bool> DeleteTranslationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var translation = await _context.Translations
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (translation == null)
            return false;

        _context.Translations.Remove(translation);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<int> BulkUpsertTranslationsAsync(BulkTranslationDto dto, CancellationToken cancellationToken = default)
    {
        var count = 0;

        foreach (var (key, value) in dto.Translations)
        {
            var existing = await _context.Translations
                .FirstOrDefaultAsync(t => t.LanguageCode == dto.LanguageCode && t.Key == key, cancellationToken);

            if (existing != null)
            {
                existing.Value = value;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                await _context.Translations.AddAsync(new TranslationEntity
                {
                    Id = Guid.NewGuid(),
                    LanguageCode = dto.LanguageCode,
                    Key = key,
                    Value = value,
                    Category = dto.Category,
                    CreatedAt = DateTime.UtcNow
                }, cancellationToken);
            }

            count++;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return count;
    }

    #endregion

    #region Mappers

    private static LanguageDto MapToDto(LanguageEntity entity)
    {
        return new LanguageDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            NativeName = entity.NativeName,
            IsActive = entity.IsActive,
            IsDefault = entity.IsDefault
        };
    }

    private static TranslationDto MapToDto(TranslationEntity entity)
    {
        return new TranslationDto
        {
            Id = entity.Id,
            LanguageCode = entity.LanguageCode,
            Key = entity.Key,
            Value = entity.Value,
            Category = entity.Category
        };
    }

    #endregion
}