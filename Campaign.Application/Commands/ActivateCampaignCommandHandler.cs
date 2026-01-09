using Campaign.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Campaign.Application.Commands;

public class ActivateCampaignCommandHandler : IRequestHandler<ActivateCampaignCommand, Result>
{
    private readonly ICampaignRepository _repository;

    public ActivateCampaignCommandHandler(ICampaignRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(ActivateCampaignCommand request, CancellationToken cancellationToken)
    {
        var campaign = await _repository.GetByIdAsync(request.CampaignId, cancellationToken);

        if (campaign == null)
            return Result.Failure("Kampanya bulunamadı", ErrorCodes.NotFound);

        // Önce onaya gönder (taslaktan aktife direkt geçiş için)
        if (campaign.Status.Name == "Draft")
        {
            var submitResult = campaign.SubmitForApproval(request.ApproverUsername);
            if (submitResult.IsFailure)
                return submitResult;
        }

        var result = campaign.Activate(request.ApproverUsername);

        if (result.IsFailure)
            return result;

        await _repository.UpdateAsync(campaign, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}