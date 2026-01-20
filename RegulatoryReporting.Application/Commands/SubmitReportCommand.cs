using CardMerchantSystem.Shared.Kernel;
using MediatR;
using RegulatoryReporting.Application.DTOs;
using RegulatoryReporting.Domain.Entities;
using RegulatoryReporting.Domain.Repositories;

namespace RegulatoryReporting.Application.Commands;

public record SubmitReportCommand(SubmitReportDto Dto, string SubmittedBy) : IRequest<Result<GeneratedReportDto>>;

public class SubmitReportCommandHandler : IRequestHandler<SubmitReportCommand, Result<GeneratedReportDto>>
{
    private readonly IGeneratedReportRepository _reportRepository;
    private readonly IReportDefinitionRepository _definitionRepository;

    public SubmitReportCommandHandler(
        IGeneratedReportRepository reportRepository,
        IReportDefinitionRepository definitionRepository)
    {
        _reportRepository = reportRepository;
        _definitionRepository = definitionRepository;
    }

    public async Task<Result<GeneratedReportDto>> Handle(SubmitReportCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var report = await _reportRepository.GetByIdWithSubmissionsAsync(dto.GeneratedReportId, cancellationToken);
        if (report is null)
            return Result.Failure<GeneratedReportDto>("Rapor bulunamadı");

        var definition = await _definitionRepository.GetByIdAsync(report.ReportDefinitionId, cancellationToken);

        var submission = ReportSubmission.Create(
            dto.GeneratedReportId,
            dto.SubmissionMethod,
            request.SubmittedBy);

        try
        {
            // Gerçek senaryoda burada FTP, API veya portal üzerinden gönderim yapılır
            var reference = await SendReportToAuthority(report, definition!, dto.SubmissionMethod);
            submission.MarkSuccess(reference, "Başarıyla gönderildi");
        }
        catch (Exception ex)
        {
            submission.MarkFailure(ex.Message);
        }

        var addResult = report.AddSubmission(submission);
        if (addResult.IsFailure)
            return Result.Failure<GeneratedReportDto>(addResult.Error);

        _reportRepository.Update(report);
        await _reportRepository.SaveChangesAsync(cancellationToken);

        return new GeneratedReportDto
        {
            Id = report.Id,
            ReportNumber = report.ReportNumber,
            ReportDefinitionId = report.ReportDefinitionId,
            ReportDefinitionName = definition?.Name ?? "",
            AuthorityDisplayName = definition?.Authority.DisplayName ?? "",
            PeriodStart = report.PeriodStart,
            PeriodEnd = report.PeriodEnd,
            FileName = report.FileName,
            FilePath = report.FilePath,
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

    private static Task<string> SendReportToAuthority(GeneratedReport report, ReportDefinition definition, string method)
    {
        // Simülasyon - gerçek senaryoda kurum API/FTP entegrasyonu
        var reference = $"{definition.Authority.Name}-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(10000, 99999)}";
        return Task.FromResult(reference);
    }
}