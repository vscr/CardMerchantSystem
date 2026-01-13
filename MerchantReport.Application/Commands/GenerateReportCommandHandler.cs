using MerchantReport.Application.DTOs;
using MerchantReport.Domain.Entities;
using MerchantReport.Domain.Repositories;
using MerchantReport.Domain.Services;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace MerchantReport.Application.Commands;

public class GenerateReportCommandHandler
    : IRequestHandler<GenerateReportCommand, Result<ReportRequestDto>>
{
    private readonly IReportRequestRepository _repository;
    private readonly IReportGeneratorService _generatorService;

    public GenerateReportCommandHandler(
        IReportRequestRepository repository,
        IReportGeneratorService generatorService)
    {
        _repository = repository;
        _generatorService = generatorService;
    }

    public async Task<Result<ReportRequestDto>> Handle(
        GenerateReportCommand request,
        CancellationToken cancellationToken)
    {
        var reportRequest = await _repository.GetByIdAsync(request.ReportRequestId, cancellationToken);
        if (reportRequest == null)
            return Result.Failure<ReportRequestDto>("Rapor talebi bulunamadı");

        // Başlat
        var startResult = reportRequest.StartGenerating();
        if (startResult.IsFailure)
            return Result.Failure<ReportRequestDto>(startResult.Error!);

        await _repository.SaveChangesAsync(cancellationToken);

        try
        {
            // Rapor oluştur
            var generateResult = await _generatorService.GenerateReportAsync(reportRequest, cancellationToken);

            if (generateResult.IsFailure)
            {
                reportRequest.MarkAsFailed(generateResult.Error!);
                await _repository.SaveChangesAsync(cancellationToken);
                return Result.Failure<ReportRequestDto>(generateResult.Error!);
            }

            var result = generateResult.Value!;

            // Tamamlandı olarak işaretle
            reportRequest.MarkAsGenerated(
                result.FileName,
                result.FilePath,
                result.FileSize,
                result.TotalTransactions,
                result.TotalAmount,
                result.TotalCommission,
                result.NetAmount);

            await _repository.SaveChangesAsync(cancellationToken);

            return MapToDto(reportRequest);
        }
        catch (Exception ex)
        {
            reportRequest.MarkAsFailed(ex.Message);
            await _repository.SaveChangesAsync(cancellationToken);
            return Result.Failure<ReportRequestDto>($"Rapor oluşturma hatası: {ex.Message}");
        }
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