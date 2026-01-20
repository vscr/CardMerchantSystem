using CardMerchantSystem.Shared.Kernel;
using Fee.Application.DTOs;
using Fee.Domain.Repositories;
using MediatR;

namespace Fee.Application.Queries;

public record GetMerchantCommissionSummaryQuery(
    string MerchantId,
    DateTime StartDate,
    DateTime EndDate) : IRequest<Result<MerchantCommissionSummaryDto>>;
public class GetMerchantCommissionSummaryQueryHandler
    : IRequestHandler<GetMerchantCommissionSummaryQuery, Result<MerchantCommissionSummaryDto>>
{
    private readonly ICommissionBreakdownRepository _repository;

    public GetMerchantCommissionSummaryQueryHandler(ICommissionBreakdownRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<MerchantCommissionSummaryDto>> Handle(
        GetMerchantCommissionSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var summary = await _repository.GetMerchantSummaryAsync(
            request.MerchantId,
            request.StartDate,
            request.EndDate,
            cancellationToken);

        return new MerchantCommissionSummaryDto
        {
            MerchantId = request.MerchantId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TransactionCount = summary.TransactionCount,
            TotalTransactionAmount = summary.TotalTransactionAmount,
            TotalCommission = summary.TotalCommission,
            TotalBankShare = summary.TotalBankShare,
            TotalInterchangeFee = summary.TotalInterchangeFee,
            TotalBKMFee = summary.TotalBKMFee,
            TotalMerchantNet = summary.TotalMerchantNet,
            AverageCommissionRate = summary.AverageCommissionRate
        };
    }
}