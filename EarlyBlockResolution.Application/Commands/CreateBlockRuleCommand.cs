using CardMerchantSystem.Shared.Kernel;
using EarlyBlockResolution.Application.DTOs;
using EarlyBlockResolution.Domain.Entities;
using EarlyBlockResolution.Domain.Enums;
using EarlyBlockResolution.Domain.Repositories;
using MediatR;

namespace EarlyBlockResolution.Application.Commands;

public record CreateBlockRuleCommand(CreateBlockRuleDto Dto) : IRequest<Result<BlockRuleDto>>;

public class CreateBlockRuleCommandHandler : IRequestHandler<CreateBlockRuleCommand, Result<BlockRuleDto>>
{
    private readonly IBlockRuleRepository _repository;

    public CreateBlockRuleCommandHandler(IBlockRuleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<BlockRuleDto>> Handle(CreateBlockRuleCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var existing = await _repository.GetByCodeAsync(dto.Code, cancellationToken);
        if (existing is not null)
            return Result.Failure<BlockRuleDto>("Bu kodla kural zaten mevcut");

        var triggerReason = Enumeration.FromId<BlockReason>(dto.TriggerReasonId);
        if (triggerReason is null)
            return Result.Failure<BlockRuleDto>("Geçersiz bloke nedeni");

        var severity = Enumeration.FromId<AlertSeverity>(dto.SeverityId);
        if (severity is null)
            return Result.Failure<BlockRuleDto>("Geçersiz önem derecesi");

        var ruleResult = BlockRule.Create(
            dto.Code,
            dto.Name,
            dto.Description,
            triggerReason,
            severity,
            dto.Priority);

        if (ruleResult.IsFailure)
            return Result.Failure<BlockRuleDto>(ruleResult.Error);

        var rule = ruleResult.Value!;

        if (dto.AmountThreshold.HasValue)
            rule.SetAmountThreshold(dto.AmountThreshold.Value);

        if (dto.CountThreshold.HasValue && dto.TimeWindowMinutes.HasValue)
            rule.SetCountThreshold(dto.CountThreshold.Value, dto.TimeWindowMinutes.Value);

        if (dto.FraudScoreThreshold.HasValue)
            rule.SetFraudScoreThreshold(dto.FraudScoreThreshold.Value);

        rule.SetAutoBlock(dto.AutoBlockEnabled, dto.BlockDurationMinutes);

        await _repository.AddAsync(rule, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(rule);
    }

    private static BlockRuleDto MapToDto(BlockRule rule)
    {
        return new BlockRuleDto
        {
            Id = rule.Id,
            Code = rule.Code,
            Name = rule.Name,
            Description = rule.Description,
            TriggerReason = rule.TriggerReason.Name,
            TriggerReasonDisplayName = rule.TriggerReason.DisplayName,
            Severity = rule.Severity.Name,
            SeverityDisplayName = rule.Severity.DisplayName,
            AmountThreshold = rule.AmountThreshold,
            CountThreshold = rule.CountThreshold,
            TimeWindowMinutes = rule.TimeWindowMinutes,
            FraudScoreThreshold = rule.FraudScoreThreshold,
            AutoBlockEnabled = rule.AutoBlockEnabled,
            BlockDurationMinutes = rule.BlockDurationMinutes,
            IsActive = rule.IsActive,
            Priority = rule.Priority,
            CreatedAt = rule.CreatedAt
        };
    }
}