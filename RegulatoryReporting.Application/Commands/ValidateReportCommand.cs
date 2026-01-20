using CardMerchantSystem.Shared.Kernel;
using MediatR;
using RegulatoryReporting.Application.DTOs;
using RegulatoryReporting.Domain.Repositories;

namespace RegulatoryReporting.Application.Commands;

public record ValidateReportCommand(Guid ReportId, string ValidatedBy) : IRequest<Result<GeneratedReportDto>>;

public class ValidateReportCommandHandler : IRequestHandler<ValidateReportCommand, Result<GeneratedReportDto>>
{
    private readonly IGeneratedReportRepository _reportRepository;
    private readonly IReportDefinitionRepository _definitionRepository;

    public ValidateReportCommandHandler(
        IGeneratedReportRepository reportRepository,
        IReportDefinitionRepository definitionRepository)
    {
        _reportRepository = reportRepository;
        _definitionRepository = definitionRepository;
    }

    public async Task<Result<GeneratedReportDto>> Handle(ValidateReportCommand request, CancellationToken cancellationToken)
    {
        var report = await _reportRepository.GetByIdAsync(request.ReportId, cancellationToken);
        if (report is null)
            return Result.Failure<GeneratedReportDto>("Rapor bulunamadı");

        var definition = await _definitionRepository.GetByIdAsync(report.ReportDefinitionId, cancellationToken);

        var validateResult = report.Validate(request.ValidatedBy);
        if (validateResult.IsFailure)
            return Result.Failure<GeneratedReportDto>(validateResult.Error);

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
}