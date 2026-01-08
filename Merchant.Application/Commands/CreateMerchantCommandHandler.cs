using Merchant.Application.DTOs;
using Merchant.Domain.Entities;
using Merchant.Domain.Enums;
using Merchant.Domain.Repositories;
using Merchant.Domain.ValueObjects;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Merchant.Application.Commands;

public class CreateMerchantCommandHandler
    : IRequestHandler<CreateMerchantCommand, Result<MerchantDto>>
{
    private readonly IMerchantRepository _repository;

    public CreateMerchantCommandHandler(IMerchantRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<MerchantDto>> Handle(
        CreateMerchantCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // 1. Vergi numarası oluştur ve doğrula
        var taxNumberResult = TaxNumber.Create(dto.TaxNumber);
        if (taxNumberResult.IsFailure)
            return Result.Failure<MerchantDto>(taxNumberResult.Error!, taxNumberResult.ErrorCode);

        // 2. Aynı vergi numarası ile kayıt var mı?
        var exists = await _repository.ExistsByTaxNumberAsync(taxNumberResult.Value!, cancellationToken);
        if (exists)
            return Result.Failure<MerchantDto>("Bu vergi numarası ile kayıtlı üye işyeri bulunmaktadır.",
                ErrorCodes.ValidationError);

        // 3. IBAN oluştur
        var ibanResult = IBAN.Create(dto.IBAN);
        if (ibanResult.IsFailure)
            return Result.Failure<MerchantDto>(ibanResult.Error!, ibanResult.ErrorCode);

        // 4. Merchant tipi bul
        var merchantType = MerchantType.FromId<MerchantType>(dto.MerchantTypeId);
        if (merchantType == null)
            return Result.Failure<MerchantDto>("Geçersiz üye işyeri tipi", ErrorCodes.ValidationError);

        // 5. Merchant oluştur
        var merchantResult = MerchantAggregate.Create(
            dto.Name,
            dto.TradeName,
            taxNumberResult.Value!,
            dto.TaxOffice,
            merchantType,
            dto.PhoneNumber,
            dto.Email,
            dto.Address,
            dto.City,
            dto.District,
            ibanResult.Value!,
            dto.CommissionRate);

        if (merchantResult.IsFailure)
            return Result.Failure<MerchantDto>(merchantResult.Error!, merchantResult.ErrorCode);

        var merchant = merchantResult.Value!;

        // 6. Kaydet
        await _repository.AddAsync(merchant, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        // 7. DTO'ya dönüştür
        return MapToDto(merchant);
    }

    private static MerchantDto MapToDto(MerchantAggregate m)
    {
        return new MerchantDto
        {
            Id = m.Id,
            MerchantCode = m.MerchantCode.Value,
            Name = m.Name,
            TradeName = m.TradeName,
            TaxNumber = m.TaxNumber.Masked,
            TaxOffice = m.TaxOffice,
            MerchantType = m.MerchantType.Name,
            Status = m.Status.Name,
            StatusDisplayName = m.Status.DisplayName,
            PhoneNumber = m.PhoneNumber,
            Email = m.Email,
            Address = m.Address,
            City = m.City,
            District = m.District,
            IBAN = m.IBAN.Masked,
            CommissionRate = m.CommissionRate,
            ContractStartDate = m.ContractStartDate,
            ContractEndDate = m.ContractEndDate,
            ApprovedBy = m.ApprovedBy,
            ApprovedAt = m.ApprovedAt,
            RejectionReason = m.RejectionReason,
            ActiveTerminalCount = m.ActiveTerminalCount,
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt
        };
    }
}