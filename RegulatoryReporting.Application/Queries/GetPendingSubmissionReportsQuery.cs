using MediatR;
using RegulatoryReporting.Application.DTOs;
using RegulatoryReporting.Domain.Entities;
using RegulatoryReporting.Domain.Repositories;

namespace RegulatoryReporting.Application.Queries;

public record GetPendingSubmissionReportsQuery() : IRequest<IReadOnlyList<GeneratedReportDto>>;

public class GetPendingSubmissionReportsQueryHandler : IRequestHandler<GetPendingSubmissionReportsQuery, IReadOnlyList<GeneratedReportDto>>
{
    private readonly IGeneratedReportRepository _reportRepository;
    private readonly IReportDefinitionRepository _definitionRepository;

    public GetPendingSubmissionReportsQueryHandler(
        IGeneratedReportRepository reportRepository,
        IReportDefinitionRepository definitionRepository)
    {
        _reportRepository = reportRepository;
        _definitionRepository = definitionRepository;
    }

    public async Task<IReadOnlyList<GeneratedReportDto>> Handle(GetPendingSubmissionReportsQuery request, CancellationToken cancellationToken)
    {
        var reports = await _reportRepository.GetPendingSubmissionAsync(cancellationToken);
        var definitions = await _definitionRepository.GetAllAsync(cancellationToken);
        var definitionDict = definitions.ToDictionary(d => d.Id);

        return reports.Select(r => MapToDto(r, definitionDict.GetValueOrDefault(r.ReportDefinitionId))).ToList();
    }

    private static GeneratedReportDto MapToDto(GeneratedReport report, ReportDefinition? definition)
    {
        return new GeneratedReportDto
        {
            Id = report.Id,
            ReportNumber = report.ReportNumber,
            ReportDefinitionId = report.ReportDefinitionId,
            ReportDefinitionName = definition?.Name ?? "",
            AuthorityDisplayName = definition?.Authority.DisplayName ?? "",
            PeriodStart = report.PeriodStart,
            PeriodEnd = report.PeriodEnd,
            FileName = report.FileName ?? "",
            FilePath = report.FilePath ?? "",
            FileSizeBytes = report.FileSizeBytes,
            FileFormat = report.FileFormat.Name,
            Status = report.Status.Name,
            StatusDisplayName = report.Status.DisplayName,
            RecordCount = report.RecordCount,
            TotalAmount = report.TotalAmount,
            GeneratedAt = report.GeneratedAt,
            ValidatedAt = report.ValidatedAt,
            ValidatedBy = report.ValidatedBy,
            ErrorMessage = report.ErrorMessage,
            CreatedAt = report.CreatedAt
        };
    }
}