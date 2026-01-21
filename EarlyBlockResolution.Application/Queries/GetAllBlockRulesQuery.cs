using EarlyBlockResolution.Application.DTOs;
using EarlyBlockResolution.Domain.Entities;
using EarlyBlockResolution.Domain.Repositories;
using MediatR;

namespace EarlyBlockResolution.Application.Queries;

public record GetAllBlockRulesQuery(bool ActiveOnly = false) : IRequest<IReadOnlyList<BlockRuleDto>>;

public class GetAllBlockRulesQueryHandler : IRequestHandler<GetAllBlockRulesQuery, IReadOnlyList<BlockRuleDto>>
{
    private readonly IBlockRuleRepository _repository;

    public GetAllBlockRulesQueryHandler(IBlockRuleRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<BlockRuleDto>> Handle(GetAllBlockRulesQuery request, CancellationToken cancellationToken)
    {
        var rules = request.ActiveOnly
            ? await _repository.GetActiveAsync(cancellationToken)
            : await _repository.GetAllAsync(cancellationToken);

        return rules.Select(MapToDto).ToList();
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