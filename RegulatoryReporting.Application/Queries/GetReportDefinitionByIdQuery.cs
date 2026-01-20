using MediatR;
using RegulatoryReporting.Application.DTOs;
using RegulatoryReporting.Domain.Entities;
using RegulatoryReporting.Domain.Repositories;

namespace RegulatoryReporting.Application.Queries;

public record GetReportDefinitionByIdQuery(Guid Id) : IRequest<ReportDefinitionDto?>;

public class GetReportDefinitionByIdQueryHandler : IRequestHandler<GetReportDefinitionByIdQuery, ReportDefinitionDto?>
{
    private readonly IReportDefinitionRepository _repository;

    public GetReportDefinitionByIdQueryHandler(IReportDefinitionRepository repository)
    {
        _repository = repository;
    }

    public async Task<ReportDefinitionDto?> Handle(GetReportDefinitionByIdQuery request, CancellationToken cancellationToken)
    {
        var definition = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (definition is null)
            return null;

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