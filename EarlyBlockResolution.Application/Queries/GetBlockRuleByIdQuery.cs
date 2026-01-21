using EarlyBlockResolution.Application.DTOs;
using EarlyBlockResolution.Domain.Entities;
using EarlyBlockResolution.Domain.Repositories;
using MediatR;

namespace EarlyBlockResolution.Application.Queries;

public record GetBlockRuleByIdQuery(Guid Id) : IRequest<BlockRuleDto?>;

public class GetBlockRuleByIdQueryHandler : IRequestHandler<GetBlockRuleByIdQuery, BlockRuleDto?>
{
    private readonly IBlockRuleRepository _repository;

    public GetBlockRuleByIdQueryHandler(IBlockRuleRepository repository)
    {
        _repository = repository;
    }

    public async Task<BlockRuleDto?> Handle(GetBlockRuleByIdQuery request, CancellationToken cancellationToken)
    {
        var rule = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (rule is null)
            return null;

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