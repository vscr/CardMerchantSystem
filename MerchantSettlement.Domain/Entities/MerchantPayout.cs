using CardMerchantSystem.Shared.Kernel;
using MerchantSettlement.Domain.Enums;

namespace MerchantSettlement.Domain.Entities;

/// <summary>
/// Üye işyeri hakediş/ödeme kaydı
/// Settlement batch'lerinden hesaplanan net tutarın ödeme kaydı
/// </summary>
public class MerchantPayout : AggregateRoot
{
    public string PayoutNumber { get; private set; } = null!;
    public string MerchantId { get; private set; } = null!;
    public string MerchantName { get; private set; } = null!;

    // Banka bilgileri
    public string BankCode { get; private set; } = null!;
    public string BankName { get; private set; } = null!;
    public string Iban { get; private set; } = null!;

    // Dönem
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }

    // Tutarlar
    public decimal GrossAmount { get; private set; }
    public decimal TotalCommission { get; private set; }
    public decimal TotalFee { get; private set; }
    public decimal WithholdingTax { get; private set; } // Stopaj
    public decimal NetAmount { get; private set; }
    public string Currency { get; private set; } = "TRY";

    // Durum
    public PayoutStatus Status { get; private set; } = null!;

    // Planlama
    public DateTime? ScheduledDate { get; private set; }
    public DateTime? PaidAt { get; private set; }

    // Banka transfer bilgileri
    public string? BankReferenceNumber { get; private set; }
    public string? TransferDescription { get; private set; }

    // Hata/Bekletme
    public string? HoldReason { get; private set; }
    public string? FailureReason { get; private set; }

    // İlişkili batch'ler
    private readonly List<Guid> _settlementBatchIds = new();
    public IReadOnlyCollection<Guid> SettlementBatchIds => _settlementBatchIds.AsReadOnly();

    private MerchantPayout() { }

    public static Result<MerchantPayout> Create(
        string merchantId,
        string merchantName,
        string bankCode,
        string bankName,
        string iban,
        DateTime periodStart,
        DateTime periodEnd,
        decimal grossAmount,
        decimal totalCommission,
        decimal totalFee,
        decimal withholdingTax = 0)
    {
        if (string.IsNullOrWhiteSpace(merchantId))
            return Result.Failure<MerchantPayout>("Merchant ID boş olamaz");

        if (string.IsNullOrWhiteSpace(iban))
            return Result.Failure<MerchantPayout>("IBAN boş olamaz");

        if (grossAmount < 0)
            return Result.Failure<MerchantPayout>("Brüt tutar negatif olamaz");

        var netAmount = grossAmount - totalCommission - totalFee - withholdingTax;

        var payout = new MerchantPayout
        {
            PayoutNumber = GeneratePayoutNumber(),
            MerchantId = merchantId,
            MerchantName = merchantName,
            BankCode = bankCode,
            BankName = bankName,
            Iban = iban.Replace(" ", "").ToUpperInvariant(),
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            GrossAmount = grossAmount,
            TotalCommission = totalCommission,
            TotalFee = totalFee,
            WithholdingTax = withholdingTax,
            NetAmount = netAmount,
            Status = PayoutStatus.Pending
        };

        return payout;
    }

    /// <summary>
    /// Settlement batch ID'si ekler
    /// </summary>
    public void AddSettlementBatch(Guid batchId)
    {
        if (!_settlementBatchIds.Contains(batchId))
            _settlementBatchIds.Add(batchId);
    }

    /// <summary>
    /// Ödemeyi planlar
    /// </summary>
    public Result Schedule(DateTime scheduledDate, string operatorUsername)
    {
        if (!Status.CanSchedule)
            return Result.Failure($"Bu durumda planlanamaz. Mevcut durum: {Status.DisplayName}");

        if (scheduledDate < DateTime.UtcNow.Date)
            return Result.Failure("Planlanan tarih geçmiş olamaz");

        Status = PayoutStatus.Scheduled;
        ScheduledDate = scheduledDate;
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Ödemeyi işleme alır
    /// </summary>
    public Result StartProcessing(string operatorUsername)
    {
        if (!Status.CanProcess)
            return Result.Failure($"Bu durumda işlenemez. Mevcut durum: {Status.DisplayName}");

        Status = PayoutStatus.Processing;
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Ödemeyi tamamlar
    /// </summary>
    public Result MarkAsPaid(string bankReferenceNumber, string operatorUsername)
    {
        if (!Status.CanPay)
            return Result.Failure($"Bu durumda ödenemez. Mevcut durum: {Status.DisplayName}");

        if (string.IsNullOrWhiteSpace(bankReferenceNumber))
            return Result.Failure("Banka referans numarası boş olamaz");

        Status = PayoutStatus.Paid;
        BankReferenceNumber = bankReferenceNumber;
        PaidAt = DateTime.UtcNow;
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Ödemeyi bekletir
    /// </summary>
    public Result PutOnHold(string reason, string operatorUsername)
    {
        if (!Status.CanHold)
            return Result.Failure($"Bu durumda bekletilemez. Mevcut durum: {Status.DisplayName}");

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure("Bekletme sebebi boş olamaz");

        Status = PayoutStatus.OnHold;
        HoldReason = reason;
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Bekletmeyi kaldırır
    /// </summary>
    public Result ReleaseHold(string operatorUsername)
    {
        if (Status != PayoutStatus.OnHold)
            return Result.Failure("Sadece bekletilen ödeme serbest bırakılabilir");

        Status = PayoutStatus.Pending;
        HoldReason = null;
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Ödemeyi başarısız olarak işaretler
    /// </summary>
    public Result MarkAsFailed(string reason, string operatorUsername)
    {
        if (Status != PayoutStatus.Processing)
            return Result.Failure("Sadece işlenmekte olan ödeme başarısız yapılabilir");

        Status = PayoutStatus.Failed;
        FailureReason = reason;
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Ödemeyi iptal eder
    /// </summary>
    public Result Cancel(string reason, string operatorUsername)
    {
        if (!Status.CanCancel)
            return Result.Failure($"Bu durumda iptal edilemez. Mevcut durum: {Status.DisplayName}");

        Status = PayoutStatus.Cancelled;
        FailureReason = reason;
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Transfer açıklamasını ayarlar
    /// </summary>
    public void SetTransferDescription(string description)
    {
        TransferDescription = description;
    }

    private static string GeneratePayoutNumber()
    {
        return $"PAY{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}