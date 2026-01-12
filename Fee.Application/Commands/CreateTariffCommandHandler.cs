using Fee.Application.DTOs;
using Fee.Domain.Entities;
using Fee.Domain.Enums;
using Fee.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Fee.Application.Commands;

public class CreateTariffCommandHandler
    : IRequestHandler<CreateTariffCommand, Result<TariffDto>>
{
    private readonly ITariffRepository _repository;

    public CreateTariffCommandHandler(ITariffRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<TariffDto>> Handle(
        CreateTariffCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Aynı kodda tarife var mı?
        var existing = await _repository.GetByCodeAsync(dto.TariffCode, cancellationToken);
        if (existing != null)
            return Result.Failure<TariffDto>("Bu kodda tarife zaten mevcut");

        // Fee type bul
        var feeType = FeeType.FromId<FeeType>(dto.FeeTypeId);
        if (feeType == null)
            return Result.Failure<TariffDto>("Geçersiz ücret tipi");

        // Tarife oluştur
        var tariffResult = Tariff.Create(
            dto.TariffCode,
            dto.TariffName,
            feeType,
            dto.EffectiveFrom,
            dto.EffectiveTo,
            dto.Description,
            dto.IsDefault);

        if (tariffResult.IsFailure)
            return Result.Failure<TariffDto>(tariffResult.Error!);

        var tariff = tariffResult.Value!;

        await _repository.AddAsync(tariff, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

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
            Rules = new List<TariffRuleDto>()
        };
    }
}