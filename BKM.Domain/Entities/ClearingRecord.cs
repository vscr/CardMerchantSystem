using BKM.Domain.Events;
using CardMerchantSystem.Shared.Kernel;

namespace BKM.Domain.Entities;

/// <summary>
/// Takas (Clearing) Kaydı
/// </summary>
public class ClearingRecord : AggregateRoot
{
    public Guid SwitchMessageId { get; private set; }
    public string STAN { get; private set; } = null!;
    public string RRN { get; private set; } = null!;
    public string AuthorizationCode { get; private set; } = null!;

    // İşlem Bilgileri
    public decimal TransactionAmount { get; private set; }
    public decimal ClearingAmount { get; private set; }
    public decimal FeeAmount { get; private set; }
    public decimal NetAmount { get; private set; }
    public string Currency { get; private set; } = null!;

    // Taraflar
    public string AcquirerBankCode { get; private set; } = null!;
    public string IssuerBankCode { get; private set; } = null!;
    public string MerchantId { get; private set; } = null!;
    public string TerminalId { get; private set; } = null!;

    // Kart Bilgileri
    public string CardNumberMasked { get; private set; } = null!;
    public string BIN { get; private set; } = null!;

    // Tarihler
    public DateTime TransactionDate { get; private set; }
    public string ClearingDate { get; private set; } = null!; // YYYYMMDD format
    public bool IsSettled { get; private set; }
    public DateTime? SettledAt { get; private set; }
    public Guid? SettlementId { get; private set; }

    // EF Core için
    private ClearingRecord() { }

    /// <summary>
    /// Clearing kaydı oluştur
    /// </summary>
    public static Result<ClearingRecord> Create(
        Guid switchMessageId,
        string stan,
        string rrn,
        string authorizationCode,
        decimal transactionAmount,
        decimal feeAmount,
        string currency,
        string acquirerBankCode,
        string issuerBankCode,
        string merchantId,
        string terminalId,
        string cardNumberMasked,
        DateTime transactionDate)
    {
        var record = new ClearingRecord
        {
            SwitchMessageId = switchMessageId,
            STAN = stan,
            RRN = rrn,
            AuthorizationCode = authorizationCode,
            TransactionAmount = transactionAmount,
            FeeAmount = feeAmount,
            ClearingAmount = transactionAmount,
            NetAmount = transactionAmount - feeAmount,
            Currency = currency,
            AcquirerBankCode = acquirerBankCode,
            IssuerBankCode = issuerBankCode,
            MerchantId = merchantId,
            TerminalId = terminalId,
            CardNumberMasked = cardNumberMasked,
            BIN = cardNumberMasked.Replace(" ", "").Substring(0, 6),
            TransactionDate = transactionDate,
            ClearingDate = DateTime.UtcNow.ToString("yyyyMMdd"),
            IsSettled = false
        };

        record.AddDomainEvent(new ClearingRecordCreatedEvent(
            record.Id,
            stan,
            transactionAmount,
            acquirerBankCode,
            issuerBankCode));

        return record;
    }

    /// <summary>
    /// Settlement'a dahil et
    /// </summary>
    public Result MarkAsSettled(Guid settlementId)
    {
        if (IsSettled)
            return Result.Failure("Kayıt zaten settle edilmiş");

        IsSettled = true;
        SettledAt = DateTime.UtcNow;
        SettlementId = settlementId;

        return Result.Success();
    }
}