using Campaign.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Campaign.Application.Commands;

public class PauseCampaignCommandHandler : IRequestHandler<PauseCampaignCommand, Result>
{
    private readonly ICampaignRepository _repository;

    public PauseCampaignCommandHandler(ICampaignRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(PauseCampaignCommand request, CancellationToken cancellationToken)
    {
        var campaign = await _repository.GetByIdAsync(request.CampaignId, cancellationToken);

        if (campaign == null)
            return Result.Failure("Kampanya bulunamadı", ErrorCodes.NotFound);

        var result = campaign.Pause(request.Reason, request.Username);

        if (result.IsFailure)
            return result;

        await _repository.UpdateAsync(campaign, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}