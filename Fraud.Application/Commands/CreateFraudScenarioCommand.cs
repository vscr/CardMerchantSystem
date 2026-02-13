using Fraud.Domain.Entities;
using Fraud.Domain.Enums;
using Fraud.Domain.Repositories;
using MediatR;

namespace Fraud.Application.Commands;

public record CreateFraudScenarioCommand(
    string Name,
    string? Description,
    Guid RuleId,
    Guid? FilterRuleId,
    FraudCheckMode CheckMode,
    string FraudResponseCode,
    int Score,
    int RunOrder,
    bool IsSimulation,
    DateTime? StartDate,
    DateTime? EndDate,
    string CreatedBy
) : IRequest<Guid>;

public class CreateFraudScenarioCommandHandler : IRequestHandler<CreateFraudScenarioCommand, Guid>
{
    private readonly IFraudScenarioRepository _repo;

    public CreateFraudScenarioCommandHandler(IFraudScenarioRepository repo)
    {
        _repo = repo;
    }

    public async Task<Guid> Handle(CreateFraudScenarioCommand cmd, CancellationToken ct)
    {
        var scenarioNo = await _repo.GetNextScenarioNoAsync(ct);

        var scenario = new FraudScenario(
            scenarioNo, cmd.Name, cmd.Description,
            cmd.RuleId, cmd.FilterRuleId,
            cmd.CheckMode, cmd.FraudResponseCode,
            cmd.Score, cmd.RunOrder, cmd.IsSimulation,
            cmd.StartDate, cmd.EndDate, cmd.CreatedBy);

        await _repo.AddAsync(scenario, ct);
        return scenario.Id;
    }
}