using Fee.Application.DTOs;
using Fee.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Fee.Application.Commands;

public class ActivateTariffCommandHandler
    : IRequestHandler<ActivateTariffCommand, Result<TariffDto>>
{
    private readonly ITariffRepository _repository;

    public ActivateTariffCommandHandler(ITariffRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<TariffDto>> Handle(
        ActivateTariffCommand request,
        CancellationToken cancellationToken)
    {
        var tariff = await _repository.GetByIdWithRulesAsync(request.TariffId, cancellationToken);
        if (tariff == null)
            return Result.Failure<TariffDto>("Tarife bulunamadı");

        var activateResult = tariff.Activate();
        if (activateResult.IsFailure)
            return Result.Failure<TariffDto>(activateResult.Error!);

        await _repository.UpdateAsync(tariff, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

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