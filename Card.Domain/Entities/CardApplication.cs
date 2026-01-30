using Card.Domain.Enums;
using Card.Domain.Events;
using Card.Domain.ValueObjects;
using CardMerchantSystem.Shared.Events;
using CardMerchantSystem.Shared.Kernel;

namespace Card.Domain.Entities;

/// <summary>
/// Kart Başvurusu Aggregate Root.
/// Başvuru oluşturulmasından kart teslimine kadar tüm süreci yönetir.
/// </summary>
public class CardApplication : AggregateRoot
{
    // Müşteri Bilgileri
    public TCKN CustomerTckn { get; private set; } = null!;
    public string CustomerName { get; private set; } = null!;
    public string CustomerSurname { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public Address DeliveryAddress { get; private set; } = null!;

    // Kart Bilgileri
    public CardType CardType { get; private set; } = null!;
    public CardApplicationStatus Status { get; private set; } = null!;
    public string? CardNumberEncrypted { get; private set; }
    public string? CardNumberMasked { get; private set; }

    // Limit Bilgileri
    public Money DailyLimit { get; private set; } = null!;
    public Money MonthlyLimit { get; private set; } = null!;

    // Basım Bilgileri
    public PrintVendor? PrintVendor { get; private set; }
    public string? PrintBatchId { get; private set; }
    public DateTime? PrintedAt { get; private set; }

    // Teslimat Bilgileri
    public string? CourierTrackingNumber { get; private set; }
    public DateTime? DeliveredAt { get; private set; }

    // Onay/Red Bilgileri
    public string? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? RejectionReason { get; private set; }
    public string? RejectedBy { get; private set; }

    // Durum Geçmişi
    private List<CardApplicationStatusHistory> _statusHistory = new();
    public IReadOnlyCollection<CardApplicationStatusHistory> StatusHistory => _statusHistory.AsReadOnly();

    // EF Core için
    private CardApplication() { }

    /// <summary>
    /// Yeni kart başvurusu oluşturur
    /// </summary>
    public static Result<CardApplication> Create(
        TCKN customerTckn,
        string customerName,
        string customerSurname,
        string phoneNumber,
        string email,
        Address deliveryAddress,
        CardType cardType,
        Money? dailyLimit = null,
        Money? monthlyLimit = null)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            return Result.Failure<CardApplication>("Müşteri adı boş olamaz");

        if (string.IsNullOrWhiteSpace(customerSurname))
            return Result.Failure<CardApplication>("Müşteri soyadı boş olamaz");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Result.Failure<CardApplication>("Telefon numarası boş olamaz");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return Result.Failure<CardApplication>("Geçerli bir e-posta adresi giriniz");

        var application = new CardApplication
        {
            CustomerTckn = customerTckn,
            CustomerName = customerName.Trim(),
            CustomerSurname = customerSurname.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            DeliveryAddress = deliveryAddress,
            CardType = cardType,
            Status = CardApplicationStatus.Pending,
            DailyLimit = dailyLimit ?? Money.TRY(cardType.DefaultDailyLimit),
            MonthlyLimit = monthlyLimit ?? Money.TRY(cardType.DefaultMonthlyLimit)
        };

        application.AddStatusHistory("Başvuru oluşturuldu", "System");

        application.AddDomainEvent(new CardApplicationCreatedEvent(
            application.Id,
            customerTckn.Value,
            cardType.Name));

        return application;
    }

    /// <summary>
    /// Başvuruyu incelemeye alır
    /// </summary>
    public Result StartReview(string reviewerUsername)
    {
        if (!Status.CanTransitionTo(CardApplicationStatus.UnderReview))
            return Result.Failure($"Bu durumda inceleme başlatılamaz. Mevcut durum: {Status.DisplayName}",
                ErrorCodes.CardApplicationInvalidStatus);

        Status = CardApplicationStatus.UnderReview;
        AddStatusHistory("İnceleme başlatıldı", reviewerUsername);
        MarkAsUpdated(reviewerUsername);

        return Result.Success();
    }

    /// <summary>
    /// Başvuruyu onaylar
    /// </summary>
    public Result Approve(string approverUsername)
    {
        if (!Status.CanTransitionTo(CardApplicationStatus.Approved))
            return Result.Failure($"Bu durumda onaylanamaz. Mevcut durum: {Status.DisplayName}",
                ErrorCodes.CardApplicationInvalidStatus);

        Status = CardApplicationStatus.Approved;
        ApprovedBy = approverUsername;
        ApprovedAt = DateTime.UtcNow;
        MarkAsUpdated(approverUsername);

        // Local event (Card modülü içinde dinlenir)
        AddDomainEvent(new CardApplicationApprovedEvent(Id, CustomerTckn.Value));

        // Integration event (Diğer modüller dinler)
        AddDomainEvent(new CardApplicationApprovedIntegrationEvent(
            Id,
            CustomerTckn.Value,
            CustomerFullName,
            CardType.Name
        ));

        return Result.Success();
    }

    /// <summary>
    /// Başvuruyu reddeder
    /// </summary>
    public Result Reject(string reason, string rejectorUsername)
    {
        if (!Status.CanTransitionTo(CardApplicationStatus.Rejected))
            return Result.Failure($"Bu durumda reddedilemez. Mevcut durum: {Status.DisplayName}",
                ErrorCodes.CardApplicationInvalidStatus);

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure("Red nedeni belirtilmeli");

        Status = CardApplicationStatus.Rejected;
        RejectionReason = reason;
        RejectedBy = rejectorUsername;
        AddStatusHistory($"Başvuru reddedildi: {reason}", rejectorUsername);
        MarkAsUpdated(rejectorUsername);

        AddDomainEvent(new CardApplicationRejectedEvent(Id, CustomerTckn.Value, reason));

        return Result.Success();
    }

    /// <summary>
    /// Kart basımı talep eder
    /// </summary>
    public Result RequestCardPrint(PrintVendor vendor, string batchId, string operatorUsername)
    {
        if (!Status.CanTransitionTo(CardApplicationStatus.CardRequested))
            return Result.Failure($"Kart basımı talep edilemez. Mevcut durum: {Status.DisplayName}",
                ErrorCodes.CardApplicationInvalidStatus);

        Status = CardApplicationStatus.CardRequested;
        PrintVendor = vendor;
        PrintBatchId = batchId;
        AddStatusHistory($"Kart basımı talep edildi. Vendor: {vendor.DisplayName}, Batch: {batchId}", operatorUsername);
        MarkAsUpdated(operatorUsername);

        AddDomainEvent(new CardPrintRequestedEvent(Id, vendor.Name, batchId));

        return Result.Success();
    }

    /// <summary>
    /// Kart basıldı olarak işaretler
    /// </summary>
    public Result MarkAsPrinted(string encryptedCardNumber, string maskedCardNumber, string operatorUsername)
    {
        if (!Status.CanTransitionTo(CardApplicationStatus.CardPrinted))
            return Result.Failure($"Kart basıldı olarak işaretlenemez. Mevcut durum: {Status.DisplayName}",
                ErrorCodes.CardApplicationInvalidStatus);

        Status = CardApplicationStatus.CardPrinted;
        CardNumberEncrypted = encryptedCardNumber;
        CardNumberMasked = maskedCardNumber;
        PrintedAt = DateTime.UtcNow;
        AddStatusHistory("Kart basıldı", operatorUsername);
        MarkAsUpdated(operatorUsername);

        // Local event
        AddDomainEvent(new CardPrintedEvent(Id, maskedCardNumber));

        // Integration event
        AddDomainEvent(new CardPrintedIntegrationEvent(Id, maskedCardNumber));

        return Result.Success();
    }

    /// <summary>
    /// Kart teslimata hazır
    /// </summary>
    public Result MarkAsReadyForDelivery(string operatorUsername)
    {
        if (!Status.CanTransitionTo(CardApplicationStatus.ReadyForDelivery))
            return Result.Failure($"Teslimata hazır olarak işaretlenemez. Mevcut durum: {Status.DisplayName}",
                ErrorCodes.CardApplicationInvalidStatus);

        Status = CardApplicationStatus.ReadyForDelivery;
        AddStatusHistory("Kart teslimata hazır", operatorUsername);
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Teslimat başladı
    /// </summary>
    public Result StartDelivery(string trackingNumber, string operatorUsername)
    {
        if (!Status.CanTransitionTo(CardApplicationStatus.InDelivery))
            return Result.Failure($"Teslimat başlatılamaz. Mevcut durum: {Status.DisplayName}",
                ErrorCodes.CardApplicationInvalidStatus);

        if (string.IsNullOrWhiteSpace(trackingNumber))
            return Result.Failure("Takip numarası belirtilmeli");

        Status = CardApplicationStatus.InDelivery;
        CourierTrackingNumber = trackingNumber;
        AddStatusHistory($"Teslimat başladı. Takip No: {trackingNumber}", operatorUsername);
        MarkAsUpdated(operatorUsername);

        AddDomainEvent(new CardDeliveryStartedEvent(Id, trackingNumber, DeliveryAddress.SingleLine));

        return Result.Success();
    }

    /// <summary>
    /// Kart teslim edildi
    /// </summary>
    public Result MarkAsDelivered(string operatorUsername)
    {
        if (!Status.CanTransitionTo(CardApplicationStatus.Delivered))
            return Result.Failure($"Teslim edildi olarak işaretlenemez. Mevcut durum: {Status.DisplayName}",
                ErrorCodes.CardApplicationInvalidStatus);

        Status = CardApplicationStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        AddStatusHistory("Kart teslim edildi", operatorUsername);
        MarkAsUpdated(operatorUsername);

        AddDomainEvent(new CardDeliveredEvent(Id, CustomerTckn.Value, CardNumberMasked!));

        return Result.Success();
    }

    /// <summary>
    /// Başvuruyu iptal eder
    /// </summary>
    public Result Cancel(string reason, string operatorUsername)
    {
        if (!Status.IsCancellable)
            return Result.Failure($"Bu durumda iptal edilemez. Mevcut durum: {Status.DisplayName}",
                ErrorCodes.CardApplicationInvalidStatus);

        Status = CardApplicationStatus.Cancelled;
        AddStatusHistory($"Başvuru iptal edildi: {reason}", operatorUsername);
        MarkAsUpdated(operatorUsername);

        AddDomainEvent(new CardApplicationCancelledEvent(Id, CustomerTckn.Value, reason));

        return Result.Success();
    }

    /// <summary>
    /// Limitleri günceller
    /// </summary>
    public Result UpdateLimits(Money dailyLimit, Money monthlyLimit, string operatorUsername)
    {
        if (dailyLimit > monthlyLimit)
            return Result.Failure("Günlük limit aylık limitten büyük olamaz");

        DailyLimit = dailyLimit;
        MonthlyLimit = monthlyLimit;
        AddStatusHistory($"Limitler güncellendi. Günlük: {dailyLimit}, Aylık: {monthlyLimit}", operatorUsername);
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    private void AddStatusHistory(string description, string changedBy)
    {
        _statusHistory.Add(new CardApplicationStatusHistory(
            Id,
            Status,
            description,
            changedBy));
    }

    public string CustomerFullName => $"{CustomerName} {CustomerSurname}";
}