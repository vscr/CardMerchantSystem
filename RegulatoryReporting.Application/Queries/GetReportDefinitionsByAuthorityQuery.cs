using CardMerchantSystem.Shared.Kernel;
using MediatR;
using RegulatoryReporting.Application.DTOs;
using RegulatoryReporting.Domain.Entities;
using RegulatoryReporting.Domain.Enums;
using RegulatoryReporting.Domain.Repositories;

namespace RegulatoryReporting.Application.Queries;

public record GetReportDefinitionsByAuthorityQuery(int AuthorityId) : IRequest<IReadOnlyList<ReportDefinitionDto>>;

public class GetReportDefinitionsByAuthorityQueryHandler : IRequestHandler<GetReportDefinitionsByAuthorityQuery, IReadOnlyList<ReportDefinitionDto>>
{
    private readonly IReportDefinitionRepository _repository;

    public GetReportDefinitionsByAuthorityQueryHandler(IReportDefinitionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ReportDefinitionDto>> Handle(GetReportDefinitionsByAuthorityQuery request, CancellationToken cancellationToken)
    {
        var authority = Enumeration.FromId<RegulatoryAuthority>(request.AuthorityId);
        if (authority is null)
            return new List<ReportDefinitionDto>();

        var definitions = await _repository.GetByAuthorityAsync(authority, cancellationToken);

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