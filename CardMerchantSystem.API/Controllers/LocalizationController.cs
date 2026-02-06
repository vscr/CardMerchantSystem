using CardMerchantSystem.API.Auth.Constants;
using CardMerchantSystem.API.Auth.Models;
using CardMerchantSystem.API.Auth.Services;
using CardMerchantSystem.Shared.Kernel.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers;

public class LocalizationController : ApiControllerBase
{
    private readonly ILocalizationService _localizationService;

    public LocalizationController(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    #region Languages

    /// <summary>
    /// Tüm dilleri listeler
    /// </summary>
    [HttpGet("languages")]
    [AllowAnonymous]
    public async Task<ActionResult<List<LanguageDto>>> GetLanguages(CancellationToken cancellationToken)
    {
        var languages = await _localizationService.GetAllLanguagesAsync(cancellationToken);
        return Ok(languages);
    }

    /// <summary>
    /// Dil detayını getirir
    /// </summary>
    [HttpGet("languages/{code}")]
    [AllowAnonymous]
    public async Task<ActionResult<LanguageDto>> GetLanguageByCode(string code, CancellationToken cancellationToken)
    {
        var language = await _localizationService.GetLanguageByCodeAsync(code, cancellationToken);
        return Ok(HandleNotFound(language, "Dil", code));
    }

    /// <summary>
    /// Yeni dil oluşturur (Admin)
    /// </summary>
    [HttpPost("languages")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult<LanguageDto>> CreateLanguage([FromBody] CreateLanguageDto dto, CancellationToken cancellationToken)
    {
        var language = await _localizationService.CreateLanguageAsync(dto, cancellationToken);

        if (language == null)
            throw new ConflictException("Bu kod ile dil zaten mevcut.");

        return Ok(language);
    }

    /// <summary>
    /// Varsayılan dili ayarlar (Admin)
    /// </summary>
    [HttpPut("languages/{code}/set-default")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult> SetDefaultLanguage(string code, CancellationToken cancellationToken)
    {
        var result = await _localizationService.SetDefaultLanguageAsync(code, cancellationToken);

        if (!result)
            throw new NotFoundException("Dil", code);

        return Ok(new { message = "Varsayılan dil güncellendi" });
    }

    /// <summary>
    /// Dil durumunu değiştirir (Admin)
    /// </summary>
    [HttpPut("languages/{code}/toggle-status")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult> ToggleLanguageStatus(string code, CancellationToken cancellationToken)
    {
        var result = await _localizationService.ToggleLanguageStatusAsync(code, cancellationToken);

        if (!result)
            throw new BusinessRuleException("Dil durumu değiştirilemedi. Varsayılan dil deaktif edilemez.");

        return Ok(new { message = "Dil durumu değiştirildi" });
    }

    #endregion

    #region Translations

    /// <summary>
    /// Belirli dildeki tüm çevirileri key-value olarak getirir
    /// </summary>
    [HttpGet("translations/{languageCode}")]
    [AllowAnonymous]
    public async Task<ActionResult<Dictionary<string, string>>> GetTranslations(string languageCode, CancellationToken cancellationToken)
    {
        var translations = await _localizationService.GetTranslationsAsync(languageCode, cancellationToken);
        return Ok(translations);
    }

    /// <summary>
    /// Belirli dil ve kategorideki çevirileri getirir
    /// </summary>
    [HttpGet("translations/{languageCode}/{category}")]
    [AllowAnonymous]
    public async Task<ActionResult<Dictionary<string, string>>> GetTranslationsByCategory(string languageCode, string category, CancellationToken cancellationToken)
    {
        var translations = await _localizationService.GetTranslationsByCategoryAsync(languageCode, category, cancellationToken);
        return Ok(translations);
    }

    /// <summary>
    /// Belirli dildeki tüm çevirileri detaylı listeler (Admin)
    /// </summary>
    [HttpGet("translations/{languageCode}/all")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult<List<TranslationDto>>> GetAllTranslations(string languageCode, CancellationToken cancellationToken)
    {
        var translations = await _localizationService.GetAllTranslationsAsync(languageCode, cancellationToken);
        return Ok(translations);
    }

    /// <summary>
    /// Yeni çeviri ekler (Admin)
    /// </summary>
    [HttpPost("translations")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult<TranslationDto>> CreateTranslation([FromBody] CreateTranslationDto dto, CancellationToken cancellationToken)
    {
        var translation = await _localizationService.CreateTranslationAsync(dto, cancellationToken);

        if (translation == null)
            throw new ConflictException("Bu key ile çeviri zaten mevcut.");

        return Ok(translation);
    }

    /// <summary>
    /// Çeviri günceller (Admin)
    /// </summary>
    [HttpPut("translations/{id:guid}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult<TranslationDto>> UpdateTranslation(Guid id, [FromBody] UpdateTranslationDto dto, CancellationToken cancellationToken)
    {
        var translation = await _localizationService.UpdateTranslationAsync(id, dto, cancellationToken);
        return Ok(HandleNotFound(translation, "Çeviri", id));
    }

    /// <summary>
    /// Çeviri siler (Admin)
    /// </summary>
    [HttpDelete("translations/{id:guid}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult> DeleteTranslation(Guid id, CancellationToken cancellationToken)
    {
        var result = await _localizationService.DeleteTranslationAsync(id, cancellationToken);

        if (!result)
            throw new NotFoundException("Çeviri", id);

        return (ActionResult)NoContent();
    }

    /// <summary>
    /// Toplu çeviri ekler/günceller (Admin)
    /// </summary>
    [HttpPost("translations/bulk")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult> BulkUpsertTranslations([FromBody] BulkTranslationDto dto, CancellationToken cancellationToken)
    {
        var count = await _localizationService.BulkUpsertTranslationsAsync(dto, cancellationToken);
        return Ok(new { message = $"{count} çeviri eklendi/güncellendi" });
    }

    #endregion
}