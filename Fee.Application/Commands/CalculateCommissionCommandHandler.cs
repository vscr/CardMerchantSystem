using Fee.Application.DTOs;
using Fee.Domain.Enums;
using Fee.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Fee.Application.Commands;

public class CalculateCommissionCommandHandler
    : IRequestHandler<CalculateCommissionCommand, Result<CalculateCommissionResponseDto>>
{
    private readonly IMerchantTariffRepository _merchantTariffRepository;
    private readonly ITariffRepository _tariffRepository;

    // Sabit oranlar (gerçek sistemde config'den alınır)
    private const decimal BankShareRate = 60m;      // %60
    private const decimal InterchangeRate = 32m;    // %32
    private const decimal BKMRate = 8m;             // %8

    public CalculateCommissionCommandHandler(
        IMerchantTariffRepository merchantTariffRepository,
        ITariffRepository tariffRepository)
    {
        _merchantTariffRepository = merchantTariffRepository;
        _tariffRepository = tariffRepository;
    }

    public async Task<Result<CalculateCommissionResponseDto>> Handle(
        CalculateCommissionCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Request;

        // Üye işyerinin tarifesini bul
        var merchantTariff = await _merchantTariffRepository.GetActiveTariffAsync(
            dto.MerchantId,
            FeeType.TransactionCommission,
            cancellationToken);

        Domain.Entities.Tariff? tariff;

        if (merchantTariff != null)
        {
            tariff = await _tariffRepository.GetByIdWithRulesAsync(merchantTariff.TariffId, cancellationToken);
        }
        else
        {
            // Default tarife
            tariff = await _tariffRepository.GetDefaultTariffAsync(FeeType.TransactionCommission, cancellationToken);
        }

        if (tariff == null)
            return Result.Failure<CalculateCommissionResponseDto>("Uygulanabilir tarife bulunamadı");

        // Komisyon hesapla
        var feeResult = tariff.CalculateFee(dto.TransactionAmount, dto.MCC, dto.InstallmentCount);

        if (feeResult.IsFailure)
            return Result.Failure<CalculateCommissionResponseDto>(feeResult.Error!);

        var fee = feeResult.Value!;

        // Özel oran varsa uygula
        var effectiveRate = merchantTariff?.SpecialRate ?? fee.Rate;
        var totalCommission = dto.TransactionAmount * effectiveRate / 100;
        totalCommission = Math.Round(totalCommission, 2);

        // Kırılım hesapla
        var bankShare = Math.Round(totalCommission * BankShareRate / 100, 2);
        var interchangeFee = Math.Round(totalCommission * InterchangeRate / 100, 2);
        var bkmFee = Math.Round(totalCommission * BKMRate / 100, 2);

        // Yuvarlama farkını banka payına ekle
        var remaining = totalCommission - bankShare - interchangeFee - bkmFee;
        bankShare += remaining;

        return new CalculateCommissionResponseDto
        {
            TransactionAmount = dto.TransactionAmount,
            TotalCommission = totalCommission,
            MerchantNetAmount = dto.TransactionAmount - totalCommission,
            CommissionRate = effectiveRate,
            TariffCode = tariff.TariffCode,
            CalculationType = fee.CalculationType,
            BankShare = bankShare,
            InterchangeFee = interchangeFee,
            BKMFee = bkmFee,
            BankShareRate = BankShareRate,
            InterchangeRate = InterchangeRate,
            BKMRate = BKMRate
        };
    }
}