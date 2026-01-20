using CardMerchantSystem.Shared.Kernel;
using BulkCardPrint.Domain.Enums;

namespace BulkCardPrint.Domain.Entities;

/// <summary>
/// Basım batch item'ı - Tek bir kart
/// </summary>
public class PrintBatchItem : Entity
{
    public Guid PrintBatchId { get; private set; }
    public Guid CardApplicationId { get; private set; }

    // Müşteri bilgileri
    public string CustomerName { get; private set; } = null!;
    public string CustomerSurname { get; private set; } = null!;
    public string CustomerTckn { get; private set; } = null!;

    // Kart bilgileri
    public string CardType { get; private set; } = null!;
    public string? CardNumberEncrypted { get; private set; }
    public string? CardNumberMasked { get; private set; }
    public string? ExpiryDate { get; private set; } // MMYY format
    public string? Cvv { get; private set; }

    // Teslimat adresi
    public string DeliveryAddress { get; private set; } = null!;

    // Durum
    public PrintItemStatus Status { get; private set; } = null!;

    // Tarihler
    public DateTime? PrintedAt { get; private set; }
    public DateTime? QualityCheckedAt { get; private set; }

    // Hata
    public string? FailureReason { get; private set; }

    // Sıra numarası
    public int SequenceNumber { get; private set; }

    private PrintBatchItem() { }

    public static PrintBatchItem Create(
        Guid printBatchId,
        Guid cardApplicationId,
        string customerName,
        string customerSurname,
        string customerTckn,
        string cardType,
        string deliveryAddress,
        int sequenceNumber)
    {
        return new PrintBatchItem
        {
            PrintBatchId = printBatchId,
            CardApplicationId = cardApplicationId,
            CustomerName = customerName,
            CustomerSurname = customerSurname,
            CustomerTckn = customerTckn,
            CardType = cardType,
            DeliveryAddress = deliveryAddress,
            SequenceNumber = sequenceNumber,
            Status = PrintItemStatus.Pending
        };
    }

    /// <summary>
    /// Kart numarası atar (HSM'den)
    /// </summary>
    public void AssignCardNumber(string encryptedNumber, string maskedNumber, string expiryDate, string cvv)
    {
        CardNumberEncrypted = encryptedNumber;
        CardNumberMasked = maskedNumber;
        ExpiryDate = expiryDate;
        Cvv = cvv;
    }

    /// <summary>
    /// Üretimi başlat
    /// </summary>
    public void StartProduction()
    {
        if (Status.CanPrint)
            Status = PrintItemStatus.InProduction;
    }

    /// <summary>
    /// Basıldı olarak işaretle
    /// </summary>
    public Result MarkAsPrinted()
    {
        if (!Status.CanPrint)
            return Result.Failure($"Bu durumda basılamaz. Mevcut durum: {Status.DisplayName}");

        Status = PrintItemStatus.Printed;
        PrintedAt = DateTime.UtcNow;
        return Result.Success();
    }

    /// <summary>
    /// Kalite kontrolü geçti
    /// </summary>
    public Result PassQualityCheck()
    {
        if (!Status.CanMarkReady)
            return Result.Failure($"Bu durumda kalite kontrolü yapılamaz. Mevcut durum: {Status.DisplayName}");

        Status = PrintItemStatus.ReadyForDelivery;
        QualityCheckedAt = DateTime.UtcNow;
        return Result.Success();
    }

    /// <summary>
    /// Kalite kontrolü başarısız
    /// </summary>
    public Result FailQualityCheck(string reason)
    {
        if (Status != PrintItemStatus.Printed)
            return Result.Failure("Sadece basılmış kartlar kalite kontrolünden geçebilir");

        Status = PrintItemStatus.QualityFailed;
        FailureReason = reason;
        QualityCheckedAt = DateTime.UtcNow;
        return Result.Success();
    }

    /// <summary>
    /// İptal et
    /// </summary>
    public Result Cancel(string reason)
    {
        if (!Status.CanCancel)
            return Result.Failure($"Bu durumda iptal edilemez. Mevcut durum: {Status.DisplayName}");

        Status = PrintItemStatus.Cancelled;
        FailureReason = reason;
        return Result.Success();
    }

    public string CustomerFullName => $"{CustomerName} {CustomerSurname}";
}