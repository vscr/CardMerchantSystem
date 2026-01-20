using CardMerchantSystem.Shared.Kernel;
using MediatR;
using RegulatoryReporting.Application.DTOs;
using RegulatoryReporting.Domain.Entities;
using RegulatoryReporting.Domain.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace RegulatoryReporting.Application.Commands;

public record GenerateReportCommand(GenerateReportDto Dto, string OperatorUsername) : IRequest<Result<GeneratedReportDto>>;

public class GenerateReportCommandHandler : IRequestHandler<GenerateReportCommand, Result<GeneratedReportDto>>
{
    private readonly IGeneratedReportRepository _reportRepository;
    private readonly IReportDefinitionRepository _definitionRepository;

    public GenerateReportCommandHandler(
        IGeneratedReportRepository reportRepository,
        IReportDefinitionRepository definitionRepository)
    {
        _reportRepository = reportRepository;
        _definitionRepository = definitionRepository;
    }

    public async Task<Result<GeneratedReportDto>> Handle(GenerateReportCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var definition = await _definitionRepository.GetByIdAsync(dto.ReportDefinitionId, cancellationToken);
        if (definition is null)
            return Result.Failure<GeneratedReportDto>("Rapor tanımı bulunamadı");

        if (!definition.IsActive)
            return Result.Failure<GeneratedReportDto>("Rapor tanımı aktif değil");

        var reportResult = GeneratedReport.Create(
            dto.ReportDefinitionId,
            dto.PeriodStart,
            dto.PeriodEnd,
            definition.FileFormat);

        if (reportResult.IsFailure)
            return Result.Failure<GeneratedReportDto>(reportResult.Error);

        var report = reportResult.Value!;

        // Üretim başlat
        report.StartGeneration();

        try
        {
            // Gerçek senaryoda burada rapor verileri sorgulanır ve dosya oluşturulur
            var (content, recordCount, totalAmount) = await GenerateReportContent(definition, dto.PeriodStart, dto.PeriodEnd);

            var fileName = $"{definition.Code}_{dto.PeriodStart:yyyyMMdd}_{dto.PeriodEnd:yyyyMMdd}{definition.FileFormat.GetFileExtension()}";
            var filePath = $"/reports/{definition.Authority.Name}/{DateTime.UtcNow:yyyy/MM}/{fileName}";
            var checksum = ComputeChecksum(content);

            report.CompleteGeneration(
                fileName,
                filePath,
                Encoding.UTF8.GetByteCount(content),
                checksum,
                recordCount,
                totalAmount);

            definition.MarkAsGenerated();
            _definitionRepository.Update(definition);
        }
        catch (Exception ex)
        {
            report.FailGeneration(ex.Message);
        }

        await _reportRepository.AddAsync(report, cancellationToken);
        await _reportRepository.SaveChangesAsync(cancellationToken);

        return new GeneratedReportDto
        {
            Id = report.Id,
            ReportNumber = report.ReportNumber,
            ReportDefinitionId = report.ReportDefinitionId,
            ReportDefinitionName = definition.Name,
            AuthorityDisplayName = definition.Authority.DisplayName,
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

    private static Task<(string Content, int RecordCount, decimal TotalAmount)> GenerateReportContent(
        ReportDefinition definition,
        DateTime periodStart,
        DateTime periodEnd)
    {
        // Örnek içerik - gerçek senaryoda veritabanından veri çekilir
        var sb = new StringBuilder();
        sb.AppendLine($"<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine($"<Report>");
        sb.AppendLine($"  <ReportCode>{definition.Code}</ReportCode>");
        sb.AppendLine($"  <Authority>{definition.Authority.Name}</Authority>");
        sb.AppendLine($"  <PeriodStart>{periodStart:yyyy-MM-dd}</PeriodStart>");
        sb.AppendLine($"  <PeriodEnd>{periodEnd:yyyy-MM-dd}</PeriodEnd>");
        sb.AppendLine($"  <GeneratedAt>{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss}</GeneratedAt>");
        sb.AppendLine($"  <Data>");
        sb.AppendLine($"    <!-- Rapor verileri -->");
        sb.AppendLine($"  </Data>");
        sb.AppendLine($"</Report>");

        return Task.FromResult((sb.ToString(), 100, 1000000m));
    }

    private static string ComputeChecksum(string content)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(content);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }
}