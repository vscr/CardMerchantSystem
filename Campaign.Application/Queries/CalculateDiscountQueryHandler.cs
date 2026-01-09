using Campaign.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Campaign.Application.Queries;

public class CalculateDiscountQueryHandler
    : IRequestHandler<CalculateDiscountQuery, Result<DiscountPreviewDto>>
{
    private readonly ICampaignRepository _repository;

    public CalculateDiscountQueryHandler(ICampaignRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<DiscountPreviewDto>> Handle(
        CalculateDiscountQuery request,
        CancellationToken cancellationToken)
    {
        var campaign = await _repository.GetByCampaignCodeAsync(request.CampaignCode, cancellationToken);

        if (campaign == null)
            return Result.Failure<DiscountPreviewDto>("Kampanya bulunamadı", ErrorCodes.NotFound);

        if (!campaign.IsCurrentlyActive)
            return Result.Failure<DiscountPreviewDto>("Kampanya aktif değil");

        var discountAmount = campaign.CalculateDiscount(request.TransactionAmount);
        var finalAmount = request.TransactionAmount - discountAmount;

        var discountDescription = campaign.DiscountType.Name == "Percentage"
            ? $"%{campaign.DiscountValue} indirim"
            : $"{campaign.DiscountValue} TL indirim";

        return new DiscountPreviewDto
        {
            CampaignCode = campaign.CampaignCode,
            CampaignName = campaign.Name,
            OriginalAmount = request.TransactionAmount,
            DiscountAmount = discountAmount,
            FinalAmount = finalAmount,
            DiscountDescription = discountDescription
        };
    }
}