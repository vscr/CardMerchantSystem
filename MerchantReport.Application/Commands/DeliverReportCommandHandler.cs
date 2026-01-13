using MerchantReport.Application.DTOs;
using MerchantReport.Domain.Entities;
using MerchantReport.Domain.Enums;
using MerchantReport.Domain.Repositories;
using MerchantReport.Domain.Services;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace MerchantReport.Application.Commands;

public class DeliverReportCommandHandler
    : IRequestHandler<DeliverReportCommand, Result<ReportRequestDto>>
{
    private readonly IReportRequestRepository _requestRepository;
    private readonly IMerchantReportConfigRepository _configRepository;
    private readonly IReportDeliveryService _deliveryService;

    public DeliverReportCommandHandler(
        IReportRequestRepository requestRepository,
        IMerchantReportConfigRepository configRepository,
        IReportDeliveryService deliveryService)
    {
        _requestRepository = requestRepository;
        _configRepository = configRepository;
        _deliveryService = deliveryService;
    }

    public async Task<Result<ReportRequestDto>> Handle(
        DeliverReportCommand request,
        CancellationToken cancellationToken)
    {
        var reportRequest = await _requestRepository.GetByIdAsync(request.ReportRequestId, cancellationToken);
        if (reportRequest == null)
            return Result.Failure<ReportRequestDto>("Rapor talebi bulunamadı");

        // Dağıtım başlat
        var startResult = reportRequest.StartDelivering();
        if (startResult.IsFailure)
            return Result.Failure<ReportRequestDto>(startResult.Error!);

        // Dağıtım yok ise direkt tamamla
        if (reportRequest.DeliveryMethod == DeliveryMethod.None)
        {
            await _requestRepository.SaveChangesAsync(cancellationToken);
            return MapToDto(reportRequest);
        }

        await _requestRepository.SaveChangesAsync(cancellationToken);

        try
        {
            // Config al
            MerchantReportConfig? config = null;
            if (reportRequest.ReportConfigId.HasValue)
            {
                config = await _configRepository.GetByIdAsync(reportRequest.ReportConfigId.Value, cancellationToken);
            }

            if (config == null)
            {
                var configs = await _configRepository.GetByMerchantIdAsync(reportRequest.MerchantId, cancellationToken);
                config = configs.FirstOrDefault();
            }

            if (config == null)
                return Result.Failure<ReportRequestDto>("Dağıtım ayarı bulunamadı");

            // Dosya içeriğini oku
            if (string.IsNullOrEmpty(reportRequest.FilePath) || !File.Exists(reportRequest.FilePath))
                return Result.Failure<ReportRequestDto>("Rapor dosyası bulunamadı");

            var fileContent = await File.ReadAllBytesAsync(reportRequest.FilePath, cancellationToken);

            // Dağıtım yap
            Result<string> deliveryResult;

            switch (reportRequest.DeliveryMethod.Name)
            {
                case nameof(DeliveryMethod.Email):
                    deliveryResult = await _deliveryService.SendViaEmailAsync(reportRequest, config, fileContent, cancellationToken);
                    break;

                case nameof(DeliveryMethod.FTP):
                case nameof(DeliveryMethod.SFTP):
                    deliveryResult = await _deliveryService.SendViaFtpAsync(reportRequest, config, fileContent, cancellationToken);
                    break;

                case nameof(DeliveryMethod.API):
                    deliveryResult = await _deliveryService.SendViaApiCallbackAsync(reportRequest, config, fileContent, cancellationToken);
                    break;

                default:
                    deliveryResult = Result.Failure<string>("Desteklenmeyen dağıtım yöntemi");
                    break;
            }

            if (deliveryResult.IsFailure)
            {
                reportRequest.MarkAsFailed(deliveryResult.Error!);
                await _requestRepository.SaveChangesAsync(cancellationToken);
                return Result.Failure<ReportRequestDto>(deliveryResult.Error!);
            }

            reportRequest.MarkAsDelivered(deliveryResult.Value);
            await _requestRepository.SaveChangesAsync(cancellationToken);

            return MapToDto(reportRequest);
        }
        catch (Exception ex)
        {
            reportRequest.MarkAsFailed(ex.Message);
            await _requestRepository.SaveChangesAsync(cancellationToken);
            return Result.Failure<ReportRequestDto>($"Dağıtım hatası: {ex.Message}");
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