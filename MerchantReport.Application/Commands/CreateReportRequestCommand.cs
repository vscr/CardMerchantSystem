using CardMerchantSystem.Shared.Kernel;
using MediatR;
using MerchantReport.Application.DTOs;
using MerchantReport.Domain.Entities;
using MerchantReport.Domain.Enums;
using MerchantReport.Domain.Repositories;

namespace MerchantReport.Application.Commands;

public record CreateReportRequestCommand(CreateReportRequestDto Dto, string? RequestedBy = null) : IRequest<Result<ReportRequestDto>>;
public class CreateReportRequestCommandHandler
    : IRequestHandler<CreateReportRequestCommand, Result<ReportRequestDto>>
{
    private readonly IReportRequestRepository _repository;

    public CreateReportRequestCommandHandler(IReportRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ReportRequestDto>> Handle(
        CreateReportRequestCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var reportType = ReportType.FromId<ReportType>(dto.ReportTypeId);
        var reportFormat = ReportFormat.FromId<ReportFormat>(dto.ReportFormatId);
        var deliveryMethod = DeliveryMethod.FromId<DeliveryMethod>(dto.DeliveryMethodId);

        if (reportType == null || reportFormat == null || deliveryMethod == null)
            return Result.Failure<ReportRequestDto>("Geçersiz enum değeri");

        var requestResult = ReportRequest.Create(
            dto.MerchantId,
            dto.MerchantName,
            reportType,
            reportFormat,
            deliveryMethod,
            dto.PeriodStart,
            dto.PeriodEnd,
            request.RequestedBy);

        if (requestResult.IsFailure)
            return Result.Failure<ReportRequestDto>(requestResult.Error!);

        var reportRequest = requestResult.Value!;

        await _repository.AddAsync(reportRequest, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(reportRequest);
    }

    private static ReportRequestDto MapToDto(ReportRequest request)
    {
        return new ReportRequestDto
        {
            Id = request.Id,
            RequestNumber = request.RequestNumber,
            MerchantId = request.MerchantId,
            MerchantName = request.MerchantName,
            ReportType = request.ReportType.Name,
            ReportTypeDisplayName = request.ReportType.DisplayName,
            ReportFormat = request.ReportFormat.Name,
            Status = request.Status.Name,
            StatusDisplayName = request.Status.DisplayName,
            PeriodStart = request.PeriodStart,
            PeriodEnd = request.PeriodEnd,
            FileName = request.FileName,
            FileSize = request.FileSize,
            DeliveryMethod = request.DeliveryMethod.Name,
            DeliveredAt = request.DeliveredAt,
            ErrorMessage = request.ErrorMessage,
            RetryCount = request.RetryCount,
            TotalTransactions = request.TotalTransactions,
            TotalAmount = request.TotalAmount,
            TotalCommission = request.TotalCommission,
            NetAmount = request.NetAmount,
            RequestedBy = request.RequestedBy,
            CreatedAt = request.CreatedAt
        };
    }
}