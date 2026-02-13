using Fraud.Domain.Enums;
using Fraud.Domain.Repositories;
using MediatR;

namespace Fraud.Application.Queries;

public record GetFraudRulesQuery : IRequest<List<FraudRuleDto>>;

public class FraudRuleDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public FraudRuleType RuleType { get; set; }
    public bool IsActive { get; set; }
    public int ConditionCount { get; set; }
    public int? PeriodMinutes { get; set; }
    public decimal? PeriodThreshold { get; set; }
    public string? PeriodFunction { get; set; }
}

public class GetFraudRulesQueryHandler : IRequestHandler<GetFraudRulesQuery, List<FraudRuleDto>>
{
    private readonly IFraudRuleRepository _repo;

    public GetFraudRulesQueryHandler(IFraudRuleRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<FraudRuleDto>> Handle(GetFraudRulesQuery query, CancellationToken ct)
    {
        var rules = await _repo.GetAllActiveAsync(ct);

        return rules.Select(r => new FraudRuleDto
        {
            Id = r.Id,
            Code = r.Code,
            Name = r.Name,
            Description = r.Description,
            RuleType = r.RuleType,
            IsActive = r.IsActive,
            ConditionCount = r.Conditions.Count,
            PeriodMinutes = r.PeriodMinutes,
            PeriodThreshold = r.PeriodThreshold,
            PeriodFunction = r.PeriodFunction
        }).ToList();
    }
}