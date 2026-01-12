using Fee.Application.DTOs;
using Fee.Domain.Entities;
using Fee.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Fee.Application.Queries;

public class GetTariffByIdQueryHandler
    : IRequestHandler<GetTariffByIdQuery, Result<TariffDto>>
{
    private readonly ITariffRepository _repository;

    public GetTariffByIdQueryHandler(ITariffRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<TariffDto>> Handle(
        GetTariffByIdQuery request,
        CancellationToken cancellationToken)
    {
        var tariff = await _repository.GetByIdWithRulesAsync(request.Id, cancellationToken);

        if (tariff == null)
            return Result.Failure<TariffDto>("Tarife bulunamadı", ErrorCodes.NotFound);

        return MapToDto(tariff);
    }

    private static TariffDto MapToDto(Tariff tariff)
    {
        return new TariffDto
        {
            Id = tariff.Id,
            TariffCode = tariff.TariffCode,
            TariffName = tariff.TariffName,
            Description = tariff.Description,
            FeeType = tariff.FeeType.Name,
            FeeTypeDisplayName = tariff.FeeType.DisplayName,
            Status = tariff.Status.Name,
            StatusDisplayName = tariff.Status.DisplayName,
            EffectiveFrom = tariff.EffectiveFrom,
            EffectiveTo = tariff.EffectiveTo,
            IsDefault = tariff.IsDefault,
            CreatedAt = tariff.CreatedAt,
            Rules = tariff.Rules.Select(r => new TariffRuleDto
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
        };
    }
}