using Fee.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace Fee.Domain.Entities;

/// <summary>
/// Ücret Tahakkuku
/// </summary>
public class FeeAccrual : AggregateRoot
{
    public string AccrualNumber { get; private set; } = null!;
    public FeeType FeeType { get; private set; } = null!;
    public AccrualStatus Status { get; private set; } = null!;
    public AccrualPeriod Period { get; private set; } = null!;

    // Kime ait
    public string? MerchantId { get; private set; }
    public string? CardNumber { get; private set; }
    public string? TerminalId { get; private set; }

    // Tutarlar
    public decimal GrossAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal NetAmount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public decimal RemainingAmount { get; private set; }

    // Tarihler
    public DateTime AccrualDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? PaidDate { get; private set; }
    public string AccrualPeriodStart { get; private set; } = null!; // YYYYMM
    public string AccrualPeriodEnd { get; private set; } = null!;   // YYYYMM

    // İlgili işlem
    public Guid? TransactionId { get; private set; }
    public Guid? TariffId { get; private set; }

    // EF Core için
    private FeeAccrual() { }

    /// <summary>
    /// Yeni tahakkuk oluşturur
    /// </summary>
    public static Result<FeeAccrual> Create(
        FeeType feeType,
        AccrualPeriod period,
        decimal grossAmount,
        DateTime dueDate,
        string periodStart,
        string periodEnd,
        string? merchantId = null,
        string? cardNumber = null,
        string? terminalId = null,
        decimal discountAmount = 0,
        Guid? transactionId = null,
        Guid? tariffId = null)
    {
        if (grossAmount <= 0)
            return Result.Failure<FeeAccrual>("Tutar sıfırdan büyük olmalı");

        var netAmount = grossAmount - discountAmount;
        if (netAmount < 0)
            return Result.Failure<FeeAccrual>("Net tutar negatif olamaz");

        var accrual = new FeeAccrual
        {
            AccrualNumber = GenerateAccrualNumber(),
            FeeType = feeType,
            Status = AccrualStatus.Pending,
            Period = period,
            MerchantId = merchantId,
            CardNumber = cardNumber,
            TerminalId = terminalId,
            GrossAmount = grossAmount,
            DiscountAmount = discountAmount,
            NetAmount = netAmount,
            PaidAmount = 0,
            RemainingAmount = netAmount,
            AccrualDate = DateTime.UtcNow,
            DueDate = dueDate,
            AccrualPeriodStart = periodStart,
            AccrualPeriodEnd = periodEnd,
            TransactionId = transactionId,
            TariffId = tariffId
        };

        return accrual;
    }

    /// <summary>
    /// Ödeme kaydı
    /// </summary>
    public Result RecordPayment(decimal amount)
    {
        if (Status.IsFinal)
            return Result.Failure("Tahakkuk zaten kapatılmış");

        if (amount <= 0)
            return Result.Failure("Ödeme tutarı sıfırdan büyük olmalı");

        if (amount > RemainingAmount)
            return Result.Failure("Ödeme tutarı kalan tutardan büyük olamaz");

        PaidAmount += amount;
        RemainingAmount = NetAmount - PaidAmount;

        if (RemainingAmount == 0)
        {
            Status = AccrualStatus.Paid;
            PaidDate = DateTime.UtcNow;
        }
        else
        {
            Status = AccrualStatus.PartiallyPaid;
        }

        return Result.Success();
    }

    /// <summary>
    /// Faturala
    /// </summary>
    public Result Invoice()
    {
        if (Status != AccrualStatus.Pending)
            return Result.Failure("Sadece bekleyen tahakkuklar faturalanabilir");

        Status = AccrualStatus.Invoiced;
        return Result.Success();
    }

    /// <summary>
    /// İptal et
    /// </summary>
    public Result Cancel(string reason)
    {
        if (Status.IsFinal)
            return Result.Failure("Tahakkuk zaten kapatılmış");

        if (PaidAmount > 0)
            return Result.Failure("Ödemesi olan tahakkuk iptal edilemez");

        Status = AccrualStatus.Cancelled;
        return Result.Success();
    }

    /// <summary>
    /// Muaf tut
    /// </summary>
    public Result Waive(string reason)
    {
        if (Status.IsFinal)
            return Result.Failure("Tahakkuk zaten kapatılmış");

        Status = AccrualStatus.Waived;
        RemainingAmount = 0;
        return Result.Success();
    }

    /// <summary>
    /// İndirim uygula
    /// </summary>
    public Result ApplyDiscount(decimal discountAmount)
    {
        if (Status.IsFinal)
            return Result.Failure("Tahakkuk zaten kapatılmış");

        if (discountAmount > NetAmount)
            return Result.Failure("İndirim net tutardan büyük olamaz");

        DiscountAmount += discountAmount;
        NetAmount = GrossAmount - DiscountAmount;
        RemainingAmount = NetAmount - PaidAmount;

        return Result.Success();
    }

    private static string GenerateAccrualNumber()
    {
        return $"ACC{DateTime.UtcNow:yyyyMMdd}{new Random().Next(100000, 999999)}";
    }
}