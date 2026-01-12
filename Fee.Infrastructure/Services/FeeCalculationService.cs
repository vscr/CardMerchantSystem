using Fee.Domain.Entities;
using Fee.Domain.Enums;
using Fee.Domain.Repositories;
using Fee.Domain.Services;
using CardMerchantSystem.Shared.Kernel;

namespace Fee.Infrastructure.Services;

public class FeeCalculationService : IFeeCalculationService
{
    private readonly ITariffRepository _tariffRepository;
    private readonly IMerchantTariffRepository _merchantTariffRepository;
    private readonly IMembershipFeeRepository _membershipFeeRepository;
    private readonly IFeeAccrualRepository _feeAccrualRepository;
    private readonly ICommissionBreakdownRepository _commissionBreakdownRepository;

    // Sabit oranlar (gerçek sistemde config'den alınır)
    private const decimal BankShareRate = 60m;
    private const decimal InterchangeRate = 32m;
    private const decimal BKMRate = 8m;

    public FeeCalculationService(
        ITariffRepository tariffRepository,
        IMerchantTariffRepository merchantTariffRepository,
        IMembershipFeeRepository membershipFeeRepository,
        IFeeAccrualRepository feeAccrualRepository,
        ICommissionBreakdownRepository commissionBreakdownRepository)
    {
        _tariffRepository = tariffRepository;
        _merchantTariffRepository = merchantTariffRepository;
        _membershipFeeRepository = membershipFeeRepository;
        _feeAccrualRepository = feeAccrualRepository;
        _commissionBreakdownRepository = commissionBreakdownRepository;
    }

    public async Task<Result<TransactionFeeResult>> CalculateTransactionFeeAsync(
        string merchantId,
        decimal transactionAmount,
        string? mcc = null,
        int installmentCount = 1,
        CancellationToken cancellationToken = default)
    {
        // Üye işyerinin tarifesini bul
        var merchantTariff = await _merchantTariffRepository.GetActiveTariffAsync(
            merchantId,
            FeeType.TransactionCommission,
            cancellationToken);

        Tariff? tariff;

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
            return Result.Failure<TransactionFeeResult>("Uygulanabilir tarife bulunamadı");

        // Komisyon hesapla
        var feeResult = tariff.CalculateFee(transactionAmount, mcc, installmentCount);

        if (feeResult.IsFailure)
            return Result.Failure<TransactionFeeResult>(feeResult.Error!);

        var fee = feeResult.Value!;

        // Özel oran varsa uygula
        var effectiveRate = merchantTariff?.SpecialRate ?? fee.Rate;
        var totalCommission = Math.Round(transactionAmount * effectiveRate / 100, 2);

        // Kırılım hesapla
        var bankShare = Math.Round(totalCommission * BankShareRate / 100, 2);
        var interchangeFee = Math.Round(totalCommission * InterchangeRate / 100, 2);
        var bkmFee = Math.Round(totalCommission * BKMRate / 100, 2);

        // Yuvarlama farkını banka payına ekle
        var remaining = totalCommission - bankShare - interchangeFee - bkmFee;
        bankShare += remaining;

        return new TransactionFeeResult
        {
            TransactionAmount = transactionAmount,
            TotalCommission = totalCommission,
            MerchantNetAmount = transactionAmount - totalCommission,
            CommissionRate = effectiveRate,
            TariffId = tariff.Id,
            TariffCode = tariff.TariffCode,
            BankShare = bankShare,
            InterchangeFee = interchangeFee,
            BKMFee = bkmFee,
            MCC = mcc,
            InstallmentCount = installmentCount,
            CalculationType = fee.CalculationType
        };
    }

    public async Task<Result<CommissionBreakdown>> CreateCommissionBreakdownAsync(
        Guid transactionId,
        string merchantId,
        decimal transactionAmount,
        string? mcc = null,
        int installmentCount = 1,
        CancellationToken cancellationToken = default)
    {
        // Komisyon hesapla
        var feeResult = await CalculateTransactionFeeAsync(
            merchantId,
            transactionAmount,
            mcc,
            installmentCount,
            cancellationToken);

        if (feeResult.IsFailure)
            return Result.Failure<CommissionBreakdown>(feeResult.Error!);

        var fee = feeResult.Value!;

        // Komisyon dağılımı oluştur
        var breakdown = CommissionBreakdown.Create(
            transactionId,
            merchantId,
            transactionAmount,
            fee.CommissionRate,
            BankShareRate,
            InterchangeRate,
            BKMRate,
            mcc,
            installmentCount,
            fee.TariffId);

        await _commissionBreakdownRepository.AddAsync(breakdown, cancellationToken);
        await _commissionBreakdownRepository.SaveChangesAsync(cancellationToken);

        return breakdown;
    }

    public async Task<Result<FeeAccrual>> CreateMembershipAccrualAsync(
        Guid membershipFeeId,
        string? merchantId = null,
        string? cardNumber = null,
        string? terminalId = null,
        CancellationToken cancellationToken = default)
    {
        var membershipFee = await _membershipFeeRepository.GetByIdAsync(membershipFeeId, cancellationToken);
        if (membershipFee == null)
            return Result.Failure<FeeAccrual>("Aidat tanımı bulunamadı");

        if (!membershipFee.IsActive)
            return Result.Failure<FeeAccrual>("Aidat tanımı aktif değil");

        // Dönem hesapla
        var now = DateTime.UtcNow;
        var periodStart = now.ToString("yyyyMM");
        var nextAccrualDate = membershipFee.Period.GetNextAccrualDate(now);
        var periodEnd = nextAccrualDate.AddDays(-1).ToString("yyyyMM");

        // Vade tarihi
        var dueDate = nextAccrualDate.AddDays(membershipFee.GracePeriodDays);

        // Tahakkuk oluştur
        var accrualResult = FeeAccrual.Create(
            membershipFee.FeeType,
            membershipFee.Period,
            membershipFee.Amount,
            dueDate,
            periodStart,
            periodEnd,
            merchantId,
            cardNumber,
            terminalId);

        if (accrualResult.IsFailure)
            return Result.Failure<FeeAccrual>(accrualResult.Error!);

        var accrual = accrualResult.Value!;

        await _feeAccrualRepository.AddAsync(accrual, cancellationToken);
        await _feeAccrualRepository.SaveChangesAsync(cancellationToken);

        return accrual;
    }

    public async Task<Result<decimal>> CalculateLateFeeAsync(
        Guid accrualId,
        CancellationToken cancellationToken = default)
    {
        var accrual = await _feeAccrualRepository.GetByIdAsync(accrualId, cancellationToken);
        if (accrual == null)
            return Result.Failure<decimal>("Tahakkuk bulunamadı");

        if (!accrual.Status.RequiresPayment)
            return Result.Failure<decimal>("Tahakkuk ödeme gerektirmiyor");

        var today = DateTime.UtcNow.Date;
        if (today <= accrual.DueDate)
            return 0m;

        var daysOverdue = (int)(today - accrual.DueDate).TotalDays;

        // Varsayılan gecikme faizi oranı: %2.5 yıllık
        const decimal defaultLateFeeRate = 2.5m;
        var dailyRate = defaultLateFeeRate / 365 / 100;
        var lateFee = Math.Round(accrual.RemainingAmount * dailyRate * daysOverdue, 2);

        return lateFee;
    }
}