using MediatR;
using RegulatoryReporting.Application.DTOs;
using RegulatoryReporting.Domain.Entities;
using RegulatoryReporting.Domain.Repositories;

namespace RegulatoryReporting.Application.Queries;

public record GetGeneratedReportByIdQuery(Guid Id, bool IncludeSubmissions = false) : IRequest<GeneratedReportDto?>;

public class GetGeneratedReportByIdQueryHandler : IRequestHandler<GetGeneratedReportByIdQuery, GeneratedReportDto?>
{
    private readonly IGeneratedReportRepository _reportRepository;
    private readonly IReportDefinitionRepository _definitionRepository;

    public GetGeneratedReportByIdQueryHandler(
        IGeneratedReportRepository reportRepository,
        IReportDefinitionRepository definitionRepository)
    {
        _reportRepository = reportRepository;
        _definitionRepository = definitionRepository;
    }

    public async Task<GeneratedReportDto?> Handle(GetGeneratedReportByIdQuery request, CancellationToken cancellationToken)
    {
        var report = request.IncludeSubmissions
            ? await _reportRepository.GetByIdWithSubmissionsAsync(request.Id, cancellationToken)
            : await _reportRepository.GetByIdAsync(request.Id, cancellationToken);

        if (report is null)
            return null;

        var definition = await _definitionRepository.GetByIdAsync(report.ReportDefinitionId, cancellationToken);

        if (request.IncludeSubmissions)
            return MapToDtoWithSubmissions(report, definition);

        return MapToDto(report, definition);
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

    private static GeneratedReportWithSubmissionsDto MapToDtoWithSubmissions(GeneratedReport report, ReportDefinition? definition)
    {
        return new GeneratedReportWithSubmissionsDto
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
            CreatedAt = report.CreatedAt,
            Submissions = report.Submissions.Select(s => new ReportSubmissionDto
            {
                Id = s.Id,
                GeneratedReportId = s.GeneratedReportId,
                SubmissionMethod = s.SubmissionMethod,
                SubmissionReference = s.SubmissionReference,
                SubmittedAt = s.SubmittedAt,
                AcknowledgedAt = s.AcknowledgedAt,
                SubmittedBy = s.SubmittedBy,
                IsSuccessful = s.IsSuccessful,
                ResponseMessage = s.ResponseMessage,
                ErrorDetails = s.ErrorDetails
            }).ToList()
        };
    }
}