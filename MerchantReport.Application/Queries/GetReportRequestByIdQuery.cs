using CardMerchantSystem.Shared.Kernel;
using MediatR;
using MerchantReport.Application.DTOs;
using MerchantReport.Domain.Repositories;

namespace MerchantReport.Application.Queries;

public record GetReportRequestByIdQuery(Guid Id) : IRequest<Result<ReportRequestDto>>;
public class GetReportRequestByIdQueryHandler
    : IRequestHandler<GetReportRequestByIdQuery, Result<ReportRequestDto>>
{
    private readonly IReportRequestRepository _repository;

    public GetReportRequestByIdQueryHandler(IReportRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ReportRequestDto>> Handle(
        GetReportRequestByIdQuery request,
        CancellationToken cancellationToken)
    {
        var reportRequest = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (reportRequest == null)
            return Result.Failure<ReportRequestDto>("Rapor talebi bulunamadı", ErrorCodes.NotFound);

        return new ReportRequestDto
        {
            Id = reportRequest.Id,
            RequestNumber = reportRequest.RequestNumber,
            MerchantId = reportRequest.MerchantId,
            MerchantName = reportRequest.MerchantName,
            ReportType = reportRequest.ReportType.Name,
            ReportTypeDisplayName = reportRequest.ReportType.DisplayName,
            ReportFormat = reportRequest.ReportFormat.Name,
            Status = reportRequest.Status.Name,
            StatusDisplayName = reportRequest.Status.DisplayName,
            PeriodStart = reportRequest.PeriodStart,
            PeriodEnd = reportRequest.PeriodEnd,
            FileName = reportRequest.FileName,
            FileSize = reportRequest.FileSize,
            DeliveryMethod = reportRequest.DeliveryMethod.Name,
            DeliveredAt = reportRequest.DeliveredAt,
            ErrorMessage = reportRequest.ErrorMessage,
            RetryCount = reportRequest.RetryCount,
            TotalTransactions = reportRequest.TotalTransactions,
            TotalAmount = reportRequest.TotalAmount,
            TotalCommission = reportRequest.TotalCommission,
            NetAmount = reportRequest.NetAmount,
            RequestedBy = reportRequest.RequestedBy,
            CreatedAt = reportRequest.CreatedAt
        };
    }
}