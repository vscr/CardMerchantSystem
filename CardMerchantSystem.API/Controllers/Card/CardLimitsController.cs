using CardMerchantSystem.API.Auth.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations;
using Transaction.Domain.Entities;
using Transaction.Domain.Repositories;
using Transaction.Domain.Services;
using Transaction.Infrastructure.Services;

namespace CardMerchantSystem.API.Controllers.Card;

[Authorize(Policy = Policies.CardManagement)]
public class CardLimitsController : ApiControllerBase
{
    private readonly ICardLimitDefinitionRepository _limitRepo;
    private readonly ILimitService _limitService;

    public CardLimitsController(
        ICardLimitDefinitionRepository limitRepo,
        ILimitService limitService)
    {
        _limitRepo = limitRepo;
        _limitService = limitService;
    }

    /// <summary>
    /// Tüm limit tanımlarını listele
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CardLimitDefinitionDto>>> GetAll(CancellationToken ct)
    {
        var definitions = await _limitRepo.GetAllAsync(ct);
        var dtos = definitions.Select(d => MapToDto(d)).ToList();
        return Ok(dtos);
    }

    /// <summary>
    /// ID ile limit tanımı getir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CardLimitDefinitionDto>> GetById(Guid id, CancellationToken ct)
    {
        var definition = await _limitRepo.GetByIdAsync(id, ct);
        return OkOrNotFound(definition != null ? MapToDto(definition) : null, "Limit tanımı", id);
    }

    /// <summary>
    /// Belirli kartın güncel limitini ve kullanımını getir
    /// </summary>
    [HttpGet("card/{maskedCardNo}/usage")]
    public async Task<ActionResult> GetCardUsage(string maskedCardNo, CancellationToken ct)
    {
        var result = await _limitService.GetCardLimitAsync(maskedCardNo, ct);
        if (result.IsFailure) return BadRequest(new { error = result.Error });

        var limit = result.Value!;
        return Ok(new
        {
            cardNumber = limit.CardNumber,
            dailyLimit = limit.DailyLimit,
            monthlyLimit = limit.MonthlyLimit,
            dailyUsed = limit.DailyUsed,
            monthlyUsed = limit.MonthlyUsed,
            remainingDaily = limit.RemainingDailyLimit,
            remainingMonthly = limit.RemainingMonthlyLimit,
            currency = limit.Currency
        });
    }

    /// <summary>
    /// Yeni limit tanımı oluştur
    /// </summary>
    [HttpPost]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult> Create(
        [FromBody] CreateLimitDefinitionRequest request, CancellationToken ct)
    {
        var definition = new CardLimitDefinition(
            request.LimitType, request.TargetValue,
            request.DailyLimit, request.MonthlyLimit,
            request.SingleTransactionLimit,
            request.Currency ?? "TRY", request.Description,
            CurrentUsername ?? "system");

        await _limitRepo.AddAsync(definition, ct);

        // Cache invalidate
        if (_limitService is LimitService ls)
            await ls.InvalidateLimitCacheAsync(request.TargetValue);

        return Ok(new { id = definition.Id, message = "Limit tanımı oluşturuldu" });
    }

    /// <summary>
    /// Limit tanımını güncelle
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult> Update(
        Guid id, [FromBody] UpdateLimitDefinitionRequest request, CancellationToken ct)
    {
        var definition = await _limitRepo.GetByIdAsync(id, ct);
        if (definition == null) return NotFound(new { error = "Limit tanımı bulunamadı" });

        definition.Update(
            request.DailyLimit, request.MonthlyLimit,
            request.SingleTransactionLimit, request.Description,
            CurrentUsername ?? "system");

        await _limitRepo.UpdateAsync(definition, ct);

        // Cache invalidate
        if (_limitService is LimitService ls)
            await ls.InvalidateLimitCacheAsync(definition.TargetValue);

        return Ok("Limit tanımı güncellendi");
    }

    /// <summary>
    /// Limit tanımını aktif/pasif yap
    /// </summary>
    [HttpPatch("{id:guid}/toggle")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult> Toggle(Guid id, CancellationToken ct)
    {
        var definition = await _limitRepo.GetByIdAsync(id, ct);
        if (definition == null) return NotFound(new { error = "Limit tanımı bulunamadı" });

        definition.SetActive(!definition.IsActive, CurrentUsername ?? "system");
        await _limitRepo.UpdateAsync(definition, ct);

        // Cache invalidate
        if (_limitService is LimitService ls)
            await ls.InvalidateLimitCacheAsync(definition.TargetValue);

        return Ok($"Limit tanımı {(definition.IsActive ? "aktif" : "pasif")} yapıldı");
    }

    /// <summary>
    /// Tüm limit cache'ini temizle
    /// </summary>
    [HttpPost("invalidate-cache")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult> InvalidateCache()
    {
        if (_limitService is LimitService ls)
            await ls.InvalidateLimitCacheAsync();

        return Ok("Limit cache temizlendi");
    }

    private static CardLimitDefinitionDto MapToDto(CardLimitDefinition d) => new()
    {
        Id = d.Id,
        LimitType = d.LimitType,
        TargetValue = d.TargetValue,
        DailyLimit = d.DailyLimit,
        MonthlyLimit = d.MonthlyLimit,
        SingleTransactionLimit = d.SingleTransactionLimit,
        Currency = d.Currency,
        IsActive = d.IsActive,
        Description = d.Description,
        CreatedAt = d.CreatedAt,
        UpdatedAt = d.UpdatedAt
    };
}

// ── DTOs ──

public class CardLimitDefinitionDto
{
    public Guid Id { get; set; }
    public string LimitType { get; set; } = null!;
    public string? TargetValue { get; set; }
    public decimal DailyLimit { get; set; }
    public decimal MonthlyLimit { get; set; }
    public decimal? SingleTransactionLimit { get; set; }
    public string Currency { get; set; } = null!;
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateLimitDefinitionRequest
{
    public string LimitType { get; set; } = null!;
    public string? TargetValue { get; set; }
    public decimal DailyLimit { get; set; }
    public decimal MonthlyLimit { get; set; }
    public decimal? SingleTransactionLimit { get; set; }
    public string? Currency { get; set; }
    public string? Description { get; set; }
}

public class UpdateLimitDefinitionRequest
{
    public decimal DailyLimit { get; set; }
    public decimal MonthlyLimit { get; set; }
    public decimal? SingleTransactionLimit { get; set; }
    public string? Description { get; set; }
}