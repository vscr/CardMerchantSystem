using MediatR;
using RegulatoryReporting.Application.DTOs;
using RegulatoryReporting.Domain.Entities;
using RegulatoryReporting.Domain.Repositories;

namespace RegulatoryReporting.Application.Queries;

public record GetAllReportDefinitionsQuery(bool ActiveOnly = false) : IRequest<IReadOnlyList<ReportDefinitionDto>>;

public class GetAllReportDefinitionsQueryHandler : IRequestHandler<GetAllReportDefinitionsQuery, IReadOnlyList<ReportDefinitionDto>>
{
    private readonly IReportDefinitionRepository _repository;

    public GetAllReportDefinitionsQueryHandler(IReportDefinitionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ReportDefinitionDto>> Handle(GetAllReportDefinitionsQuery request, CancellationToken cancellationToken)
    {
        var definitions = request.ActiveOnly
            ? await _repository.GetActiveAsync(cancellationToken)
            : await _repository.GetAllAsync(cancellationToken);

        return definitions.Select(MapToDto).ToList();
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