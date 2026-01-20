using Campaign.Domain.Repositories;
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
public class AddCampaignRuleCommandHandler : IRequestHandler<AddCampaignRuleCommand, Result>
{
    private readonly ICampaignRepository _repository;

    public AddCampaignRuleCommandHandler(ICampaignRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(AddCampaignRuleCommand request, CancellationToken cancellationToken)
    {
        var campaign = await _repository.GetByIdWithDetailsAsync(request.CampaignId, cancellationToken);

        if (campaign == null)
            return Result.Failure("Kampanya bulunamadı", ErrorCodes.NotFound);

        var result = campaign.AddRule(request.RuleName, request.RuleType, request.Operator, request.Value);

        if (result.IsFailure)
            return result;

        await _repository.UpdateAsync(campaign, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}