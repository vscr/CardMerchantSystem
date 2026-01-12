using Fee.Application.DTOs;
using Fee.Domain.Entities;
using Fee.Domain.Enums;
using Fee.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Fee.Application.Commands;

public class AssignMerchantTariffCommandHandler
    : IRequestHandler<AssignMerchantTariffCommand, Result<MerchantTariffDto>>
{
    private readonly IMerchantTariffRepository _merchantTariffRepository;
    private readonly ITariffRepository _tariffRepository;

    public AssignMerchantTariffCommandHandler(
        IMerchantTariffRepository merchantTariffRepository,
        ITariffRepository tariffRepository)
    {
        _merchantTariffRepository = merchantTariffRepository;
        _tariffRepository = tariffRepository;
    }

    public async Task<Result<MerchantTariffDto>> Handle(
        AssignMerchantTariffCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Fee type bul
        var feeType = FeeType.FromId<FeeType>(dto.FeeTypeId);
        if (feeType == null)
            return Result.Failure<MerchantTariffDto>("Geçersiz ücret tipi");

        // Tarife bul
        var tariff = await _tariffRepository.GetByIdAsync(dto.TariffId, cancellationToken);
        if (tariff == null)
            return Result.Failure<MerchantTariffDto>("Tarife bulunamadı");

        if (!tariff.Status.IsUsable)
            return Result.Failure<MerchantTariffDto>("Tarife aktif değil");

        // Mevcut aktif tarife var mı?
        var existingTariff = await _merchantTariffRepository.GetActiveTariffAsync(
            dto.MerchantId, feeType, cancellationToken);

        if (existingTariff != null)
        {
            existingTariff.Terminate();
            await _merchantTariffRepository.UpdateAsync(existingTariff, cancellationToken);
        }

        // Yeni atama oluştur
        var merchantTariffResult = MerchantTariff.Create(
            dto.MerchantId,
            dto.TariffId,
            feeType,
            dto.SpecialRate,
            dto.Notes);

        if (merchantTariffResult.IsFailure)
            return Result.Failure<MerchantTariffDto>(merchantTariffResult.Error!);

        var merchantTariff = merchantTariffResult.Value!;

        await _merchantTariffRepository.AddAsync(merchantTariff, cancellationToken);
        await _merchantTariffRepository.SaveChangesAsync(cancellationToken);

        return new MerchantTariffDto
        {
            Id = merchantTariff.Id,
            MerchantId = merchantTariff.MerchantId,
            TariffId = merchantTariff.TariffId,
            TariffCode = tariff.TariffCode,
            TariffName = tariff.TariffName,
            FeeType = merchantTariff.FeeType.Name,
            FeeTypeDisplayName = merchantTariff.FeeType.DisplayName,
            AssignedDate = merchantTariff.AssignedDate,
            EndDate = merchantTariff.EndDate,
            IsActive = merchantTariff.IsActive,
            SpecialRate = merchantTariff.SpecialRate,
            Notes = merchantTariff.Notes
        };
    }
}