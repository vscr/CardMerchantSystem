using Fraud.Domain.Enums;
using Fraud.Domain.Repositories;
using MediatR;

namespace Fraud.Application.Queries;

public record GetFraudScenariosQuery : IRequest<List<FraudScenarioDto>>;

public class FraudScenarioDto
{
    public Guid Id { get; set; }
    public int ScenarioNo { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public Guid RuleId { get; set; }
    public string? RuleName { get; set; }
    public Guid? FilterRuleId { get; set; }
    public FraudCheckMode CheckMode { get; set; }
    public string FraudResponseCode { get; set; } = null!;
    public int Score { get; set; }
    public int RunOrder { get; set; }
    public bool IsSimulation { get; set; }
    public bool IsActive { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class GetFraudScenariosQueryHandler : IRequestHandler<GetFraudScenariosQuery, List<FraudScenarioDto>>
{
    private readonly IFraudScenarioRepository _scenarioRepo;
    private readonly IFraudRuleRepository _ruleRepo;

    public GetFraudScenariosQueryHandler(
        IFraudScenarioRepository scenarioRepo,
        IFraudRuleRepository ruleRepo)
    {
        _scenarioRepo = scenarioRepo;
        _ruleRepo = ruleRepo;
    }

    public async Task<List<FraudScenarioDto>> Handle(GetFraudScenariosQuery query, CancellationToken ct)
    {
        var scenarios = await _scenarioRepo.GetAllAsync(ct);
        var rules = await _ruleRepo.GetAllActiveAsync(ct);
        var ruleLookup = rules.ToDictionary(r => r.Id, r => r.Name);

        return scenarios.Select(s => new FraudScenarioDto
        {
            Id = s.Id,
            ScenarioNo = s.ScenarioNo,
            Name = s.Name,
            Description = s.Description,
            RuleId = s.RuleId,
            RuleName = ruleLookup.GetValueOrDefault(s.RuleId, "Bilinmeyen"),
            FilterRuleId = s.FilterRuleId,
            CheckMode = s.CheckMode,
            FraudResponseCode = s.FraudResponseCode,
            Score = s.Score,
            RunOrder = s.RunOrder,
            IsSimulation = s.IsSimulation,
            IsActive = s.IsActive,
            StartDate = s.StartDate,
            EndDate = s.EndDate
        }).OrderBy(s => s.RunOrder).ToList();
    }
}