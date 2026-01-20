using CardMerchantSystem.Shared.Kernel;
using MediatR;
using RegulatoryReporting.Application.DTOs;
using RegulatoryReporting.Domain.Entities;
using RegulatoryReporting.Domain.Enums;
using RegulatoryReporting.Domain.Repositories;

namespace RegulatoryReporting.Application.Commands;

public record CreateReportDefinitionCommand(CreateReportDefinitionDto Dto) : IRequest<Result<ReportDefinitionDto>>;

public class CreateReportDefinitionCommandHandler : IRequestHandler<CreateReportDefinitionCommand, Result<ReportDefinitionDto>>
{
    private readonly IReportDefinitionRepository _repository;

    public CreateReportDefinitionCommandHandler(IReportDefinitionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ReportDefinitionDto>> Handle(CreateReportDefinitionCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var existing = await _repository.GetByCodeAsync(dto.Code, cancellationToken);
        if (existing is not null)
            return Result.Failure<ReportDefinitionDto>("Bu kodla rapor tanımı zaten mevcut");

        var authority = Enumeration.FromId<RegulatoryAuthority>(dto.AuthorityId);
        if (authority is null)
            return Result.Failure<ReportDefinitionDto>("Geçersiz kurum");

        var reportType = Enumeration.FromId<ReportType>(dto.ReportTypeId);
        if (reportType is null)
            return Result.Failure<ReportDefinitionDto>("Geçersiz rapor tipi");

        var fileFormat = Enumeration.FromId<ReportFileFormat>(dto.FileFormatId);
        if (fileFormat is null)
            return Result.Failure<ReportDefinitionDto>("Geçersiz dosya formatı");

        var frequency = Enumeration.FromId<ReportFrequency>(dto.FrequencyId);
        if (frequency is null)
            return Result.Failure<ReportDefinitionDto>("Geçersiz periyot");

        var definitionResult = ReportDefinition.Create(
            dto.Code,
            dto.Name,
            dto.Description,
            authority,
            reportType,
            fileFormat,
            frequency);

        if (definitionResult.IsFailure)
            return Result.Failure<ReportDefinitionDto>(definitionResult.Error);

        var definition = definitionResult.Value!;

        if (!string.IsNullOrWhiteSpace(dto.TemplateQuery))
            definition.SetTemplateQuery(dto.TemplateQuery);

        if (!string.IsNullOrWhiteSpace(dto.TemplateFilePath))
            definition.SetTemplateFile(dto.TemplateFilePath);

        await _repository.AddAsync(definition, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(definition);
    }

    private static ReportDefinitionDto MapToDto(ReportDefinition definition)
    {
        return new ReportDefinitionDto
        {
            Id = definition.Id,
            Code = definition.Code,
            Name = definition.Name,
            Description = definition.Description,
            Authority = definition.Authority.Name,
            AuthorityDisplayName = definition.Authority.DisplayName,
            ReportType = definition.ReportType.Name,
            ReportTypeDisplayName = definition.ReportType.DisplayName,
            FileFormat = definition.FileFormat.Name,
            FileFormatDisplayName = definition.FileFormat.DisplayName,
            Frequency = definition.Frequency.Name,
            FrequencyDisplayName = definition.Frequency.DisplayName,
            IsActive = definition.IsActive,
            LastGeneratedAt = definition.LastGeneratedAt,
            NextScheduledAt = definition.NextScheduledAt,
            CreatedAt = definition.CreatedAt
        };
    }
}