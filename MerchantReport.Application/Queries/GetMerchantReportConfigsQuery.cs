using MediatR;
using MerchantReport.Application.DTOs;
using MerchantReport.Domain.Repositories;

namespace MerchantReport.Application.Queries;

public record GetMerchantReportConfigsQuery(string MerchantId) : IRequest<IReadOnlyList<MerchantReportConfigDto>>;
public class GetMerchantReportConfigsQueryHandler
    : IRequestHandler<GetMerchantReportConfigsQuery, IReadOnlyList<MerchantReportConfigDto>>
{
    private readonly IMerchantReportConfigRepository _repository;

    public GetMerchantReportConfigsQueryHandler(IMerchantReportConfigRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<MerchantReportConfigDto>> Handle(
        GetMerchantReportConfigsQuery request,
        CancellationToken cancellationToken)
    {
        var configs = await _repository.GetByMerchantIdAsync(request.MerchantId, cancellationToken);

        return configs.Select(c => new MerchantReportConfigDto
        {
            Id = c.Id,
            MerchantId = c.MerchantId,
            MerchantName = c.MerchantName,
            ReportType = c.ReportType.Name,
            ReportTypeDisplayName = c.ReportType.DisplayName,
            ReportFormat = c.ReportFormat.Name,
            DeliveryMethod = c.DeliveryMethod.Name,
            DeliveryMethodDisplayName = c.DeliveryMethod.DisplayName,
            Frequency = c.Frequency.Name,
            FrequencyDisplayName = c.Frequency.DisplayName,
            DayOfWeek = c.DayOfWeek,
            DayOfMonth = c.DayOfMonth,
            RunTime = c.RunTime.ToString(@"hh\:mm"),
            NextRunTime = c.NextRunTime,
            LastRunTime = c.LastRunTime,
            EmailRecipients = c.EmailRecipients,
            FtpHost = c.FtpHost,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        }).ToList();
    }
}