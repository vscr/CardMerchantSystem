using CardMerchantSystem.Shared.Kernel;
using Fee.Application.DTOs;
using Fee.Domain.Enums;
using Fee.Domain.Repositories;
using MediatR;

namespace Fee.Application.Commands;

public record AddTariffRuleCommand(AddTariffRuleDto Dto) : IRequest<Result<TariffDto>>;
public class AddTariffRuleCommandHandler
    : IRequestHandler<AddTariffRuleCommand, Result<TariffDto>>
{
    private readonly ITariffRepository _repository;

    public AddTariffRuleCommandHandler(ITariffRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<TariffDto>> Handle(
        AddTariffRuleCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Tarife bul
        var tariff = await _repository.GetByIdWithRulesAsync(dto.TariffId, cancellationToken);
        if (tariff == null)
            return Result.Failure<TariffDto>("Tarife bulunamadı");

        // Calculation type bul
        var calculationType = CalculationType.FromId<CalculationType>(dto.CalculationTypeId);
        if (calculationType == null)
            return Result.Failure<TariffDto>("Geçersiz hesaplama tipi");

        // Kural ekle
        var ruleResult = tariff.AddRule(
            calculationType,
            dto.Rate,
            dto.MinimumFee,
            dto.MaximumFee,
            dto.MCC,
            dto.InstallmentCount,
            dto.VolumeFrom,
            dto.VolumeTo);

        if (ruleResult.IsFailure)
            return Result.Failure<TariffDto>(ruleResult.Error!);

        await _repository.UpdateAsync(tariff, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(tariff);
    }

    private static TariffDto MapToDto(Domain.Entities.Tariff tariff)
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