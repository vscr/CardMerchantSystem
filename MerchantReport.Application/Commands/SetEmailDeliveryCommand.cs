using CardMerchantSystem.Shared.Kernel;
using MediatR;
using MerchantReport.Application.DTOs;
using MerchantReport.Domain.Repositories;

namespace MerchantReport.Application.Commands;

public record SetEmailDeliveryCommand(SetEmailDeliveryDto Dto) : IRequest<Result<MerchantReportConfigDto>>;
public class SetEmailDeliveryCommandHandler
    : IRequestHandler<SetEmailDeliveryCommand, Result<MerchantReportConfigDto>>
{
    private readonly IMerchantReportConfigRepository _repository;

    public SetEmailDeliveryCommandHandler(IMerchantReportConfigRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<MerchantReportConfigDto>> Handle(
        SetEmailDeliveryCommand request,
        CancellationToken cancellationToken)
    {
        var config = await _repository.GetByIdAsync(request.Dto.ConfigId, cancellationToken);
        if (config == null)
            return Result.Failure<MerchantReportConfigDto>("Rapor ayarı bulunamadı");

        config.SetEmailDelivery(request.Dto.Recipients);

        await _repository.UpdateAsync(config, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new MerchantReportConfigDto
        {
            Id = config.Id,
            MerchantId = config.MerchantId,
            MerchantName = config.MerchantName,
            ReportType = config.ReportType.Name,
            ReportTypeDisplayName = config.ReportType.DisplayName,
            ReportFormat = config.ReportFormat.Name,
            DeliveryMethod = config.DeliveryMethod.Name,
            DeliveryMethodDisplayName = config.DeliveryMethod.DisplayName,
            Frequency = config.Frequency.Name,
            FrequencyDisplayName = config.Frequency.DisplayName,
            DayOfWeek = config.DayOfWeek,
            DayOfMonth = config.DayOfMonth,
            RunTime = config.RunTime.ToString(@"hh\:mm"),
            NextRunTime = config.NextRunTime,
            LastRunTime = config.LastRunTime,
            EmailRecipients = config.EmailRecipients,
            FtpHost = config.FtpHost,
            IsActive = config.IsActive,
            CreatedAt = config.CreatedAt
        };
    }
}