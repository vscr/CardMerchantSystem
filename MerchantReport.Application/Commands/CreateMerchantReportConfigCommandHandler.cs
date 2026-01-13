using MerchantReport.Application.DTOs;
using MerchantReport.Domain.Entities;
using MerchantReport.Domain.Enums;
using MerchantReport.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace MerchantReport.Application.Commands;

public class CreateMerchantReportConfigCommandHandler
    : IRequestHandler<CreateMerchantReportConfigCommand, Result<MerchantReportConfigDto>>
{
    private readonly IMerchantReportConfigRepository _repository;

    public CreateMerchantReportConfigCommandHandler(IMerchantReportConfigRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<MerchantReportConfigDto>> Handle(
        CreateMerchantReportConfigCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var reportType = ReportType.FromId<ReportType>(dto.ReportTypeId);
        var reportFormat = ReportFormat.FromId<ReportFormat>(dto.ReportFormatId);
        var deliveryMethod = DeliveryMethod.FromId<DeliveryMethod>(dto.DeliveryMethodId);
        var frequency = ScheduleFrequency.FromId<ScheduleFrequency>(dto.FrequencyId);

        if (reportType == null || reportFormat == null || deliveryMethod == null || frequency == null)
            return Result.Failure<MerchantReportConfigDto>("Geçersiz enum değeri");

        if (!TimeSpan.TryParse(dto.RunTime, out var runTime))
            return Result.Failure<MerchantReportConfigDto>("Geçersiz çalışma saati formatı");

        var configResult = MerchantReportConfig.Create(
            dto.MerchantId,
            dto.MerchantName,
            reportType,
            reportFormat,
            deliveryMethod,
            frequency,
            runTime,
            dto.DayOfWeek,
            dto.DayOfMonth);

        if (configResult.IsFailure)
            return Result.Failure<MerchantReportConfigDto>(configResult.Error!);

        var config = configResult.Value!;

        await _repository.AddAsync(config, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(config);
    }

    private static MerchantReportConfigDto MapToDto(MerchantReportConfig config)
    {
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