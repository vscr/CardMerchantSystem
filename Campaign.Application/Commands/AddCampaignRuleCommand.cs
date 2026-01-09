using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Campaign.Application.Commands;

public record AddCampaignRuleCommand(
    Guid CampaignId,
    string RuleName,
    string RuleType,
    string Operator,
    string Value
) : IRequest<Result>;