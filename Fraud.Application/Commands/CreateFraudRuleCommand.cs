using Fraud.Domain.Entities;
using Fraud.Domain.Enums;
using Fraud.Domain.Repositories;
using MediatR;

namespace Fraud.Application.Commands;

public record CreateFraudRuleCommand(
    string Code,
    string Name,
    string? Description,
    FraudRuleType RuleType,
    LogicalOperator ConditionOperator,
    string CreatedBy,
    int? PeriodMinutes,
    decimal? PeriodThreshold,
    string? PeriodFunction,
    string? PeriodGroupBy,
    string? SqlScript,
    List<CreateRuleConditionDto>? Conditions
) : IRequest<Guid>;

public record CreateRuleConditionDto(
    string ParameterName,
    RuleOperator Operator,
    string Value,
    string? SecondValue,
    int OrderIndex
);

public class CreateFraudRuleCommandHandler : IRequestHandler<CreateFraudRuleCommand, Guid>
{
    private readonly IFraudRuleRepository _repo;

    public CreateFraudRuleCommandHandler(IFraudRuleRepository repo)
    {
        _repo = repo;
    }

    public async Task<Guid> Handle(CreateFraudRuleCommand cmd, CancellationToken ct)
    {
        FraudRule rule = cmd.RuleType switch
        {
            FraudRuleType.Periodic => FraudRule.CreatePeriodic(
                cmd.Code, cmd.Name, cmd.Description,
                cmd.PeriodMinutes ?? 60,
                cmd.PeriodThreshold ?? 5,
                cmd.PeriodFunction ?? "COUNT",
                cmd.PeriodGroupBy ?? "CARD",
                cmd.CreatedBy),

            FraudRuleType.Linked => FraudRule.CreateLinked(
                cmd.Code, cmd.Name, cmd.Description,
                cmd.SqlScript ?? throw new ArgumentException("Linked rule için SqlScript gerekli"),
                cmd.CreatedBy),

            _ => new FraudRule(
                cmd.Code, cmd.Name, cmd.Description,
                cmd.RuleType, cmd.ConditionOperator, cmd.CreatedBy)
        };

        if (cmd.Conditions != null)
        {
            foreach (var c in cmd.Conditions)
            {
                var condition = new FraudRuleCondition(
                    rule.Id, c.ParameterName, c.Operator,
                    c.Value, c.SecondValue, c.OrderIndex);

                rule.AddCondition(condition);
            }
        }

        await _repo.AddAsync(rule, ct);
        return rule.Id;
    }
}