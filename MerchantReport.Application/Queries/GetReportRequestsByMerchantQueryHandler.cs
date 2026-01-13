using MerchantReport.Application.DTOs;
using MerchantReport.Domain.Repositories;
using MediatR;

namespace MerchantReport.Application.Queries;

public class GetReportRequestsByMerchantQueryHandler
    : IRequestHandler<GetReportRequestsByMerchantQuery, IReadOnlyList<ReportRequestSummaryDto>>
{
    private readonly IReportRequestRepository _repository;

    public GetReportRequestsByMerchantQueryHandler(IReportRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ReportRequestSummaryDto>> Handle(
        GetReportRequestsByMerchantQuery request,
        CancellationToken cancellationToken)
    {
        var requests = await _repository.GetByMerchantIdAsync(request.MerchantId, cancellationToken);

        return requests.Select(r => new ReportRequestSummaryDto
        {
            Id = r.Id,
            RequestNumber = r.RequestNumber,
            MerchantName = r.MerchantName,
            ReportTypeDisplayName = r.ReportType.DisplayName,
            StatusDisplayName = r.Status.DisplayName,
            PeriodStart = r.PeriodStart,
            PeriodEnd = r.PeriodEnd,
            TotalAmount = r.TotalAmount,
            CreatedAt = r.CreatedAt
        }).ToList();
    }
}