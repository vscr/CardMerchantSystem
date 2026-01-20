using Fee.Application.DTOs;
using Fee.Domain.Repositories;
using MediatR;

namespace Fee.Application.Queries;

public record GetAllTariffsQuery() : IRequest<IReadOnlyList<TariffDto>>;
public class GetAllTariffsQueryHandler
    : IRequestHandler<GetAllTariffsQuery, IReadOnlyList<TariffDto>>
{
    private readonly ITariffRepository _repository;

    public GetAllTariffsQueryHandler(ITariffRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TariffDto>> Handle(
        GetAllTariffsQuery request,
        CancellationToken cancellationToken)
    {
        var tariffs = await _repository.GetAllActiveAsync(cancellationToken);

        return tariffs.Select(t => new TariffDto
        {
            Id = t.Id,
            TariffCode = t.TariffCode,
            TariffName = t.TariffName,
            Description = t.Description,
            FeeType = t.FeeType.Name,
            FeeTypeDisplayName = t.FeeType.DisplayName,
            Status = t.Status.Name,
            StatusDisplayName = t.Status.DisplayName,
            EffectiveFrom = t.EffectiveFrom,
            EffectiveTo = t.EffectiveTo,
            IsDefault = t.IsDefault,
            CreatedAt = t.CreatedAt,
            Rules = t.Rules.Select(r => new TariffRuleDto
            {
                Id = r.Id,
                CalculationType = r.CalculationType.Name,
                CalculationTypeDisplayName = r.CalculationType.DisplayName,
                Rate = r.Rate,
                MinimumFee = r.MinimumFee,
                MaximumFee = r.MaximumFee,
                MCC = r.MCC,
                InstallmentCount = r.InstallmentCount,
                VolumeFrom = r.VolumeFrom,
                VolumeTo = r.VolumeTo,
                IsActive = r.IsActive,
                Priority = r.Priority
            }).ToList()
        }).ToList();
    }
}