using Dispute.Domain.Enums;
using Dispute.Domain.Events;
using CardMerchantSystem.Shared.Kernel;

namespace Dispute.Domain.Entities;

/// <summary>
/// İtiraz Aggregate Root
/// </summary>
public class DisputeAggregate : AggregateRoot
{
    // İtiraz Bilgileri
    public string DisputeNumber { get; private set; } = null!;
    public DisputeStatus Status { get; private set; } = null!;
    public DisputeReason Reason { get; private set; } = null!;
    public DisputePriority Priority { get; private set; } = null!;

    // İşlem Bilgileri
    public Guid TransactionId { get; private set; }
    public string TransactionReference { get; private set; } = null!;
    public decimal TransactionAmount { get; private set; }
    public decimal DisputedAmount { get; private set; }
    public DateTime TransactionDate { get; private set; }

    // Müşteri Bilgileri
    public string CustomerTckn { get; private set; } = null!;
    public string CustomerName { get; private set; } = null!;
    public string CustomerPhone { get; private set; } = null!;
    public string CustomerEmail { get; private set; } = null!;

    // Üye İşyeri Bilgileri
    public Guid MerchantId { get; private set; }
    public string MerchantCode { get; private set; } = null!;
    public string MerchantName { get; private set; } = null!;

    // İtiraz Detayları
    public string Description { get; private set; } = null!;
    public string? CustomerStatement { get; private set; }
    public string? MerchantResponse { get; private set; }
    public DateTime? MerchantResponseDate { get; private set; }

    // Atama ve Çözüm
    public string? AssignedTo { get; private set; }
    public DateTime? AssignedAt { get; private set; }
    public string? Resolution { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public string? ResolvedBy { get; private set; }
    public decimal? RefundAmount { get; private set; }

    // Son Tarihler
    public DateTime DueDate { get; private set; }
    public DateTime? EscalatedAt { get; private set; }

    // Alt Koleksiyonlar
    private readonly List<DisputeDocument> _documents = new();
    public IReadOnlyCollection<DisputeDocument> Documents => _documents.AsReadOnly();

    private readonly List<DisputeNote> _notes = new();
    public IReadOnlyCollection<DisputeNote> Notes => _notes.AsReadOnly();

    // EF Core için
    private DisputeAggregate() { }

    /// <summary>
    /// Yeni itiraz oluşturur
    /// </summary>
    public static Result<DisputeAggregate> Create(
        Guid transactionId,
        string transactionReference,
        decimal transactionAmount,
        decimal disputedAmount,
        DateTime transactionDate,
        DisputeReason reason,
        string description,
        string customerTckn,
        string customerName,
        string customerPhone,
        string customerEmail,
        Guid merchantId,
        string merchantCode,
        string merchantName)
    {
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<DisputeAggregate>("İtiraz açıklaması boş olamaz");

        if (disputedAmount <= 0)
            return Result.Failure<DisputeAggregate>("İtiraz tutarı sıfırdan büyük olmalı");

        if (disputedAmount > transactionAmount)
            return Result.Failure<DisputeAggregate>("İtiraz tutarı işlem tutarından büyük olamaz");

        var dispute = new DisputeAggregate
        {
            DisputeNumber = GenerateDisputeNumber(),
            Status = DisputeStatus.Pending,
            Reason = reason,
            Priority = DisputePriority.FromAmount(disputedAmount),
            TransactionId = transactionId,
            TransactionReference = transactionReference,
            TransactionAmount = transactionAmount,
            DisputedAmount = disputedAmount,
            TransactionDate = transactionDate,
            Description = description,
            CustomerTckn = customerTckn,
            CustomerName = customerName,
            CustomerPhone = customerPhone,
            CustomerEmail = customerEmail,
            MerchantId = merchantId,
            MerchantCode = merchantCode,
            MerchantName = merchantName,
            DueDate = DateTime.UtcNow.AddDays(reason.MaxResolutionDays)
        };

        dispute.AddDomainEvent(new DisputeCreatedEvent(
            dispute.Id,
            dispute.DisputeNumber,
            transactionId,
            disputedAmount,
            reason.DisplayName));

        return dispute;
    }

    /// <summary>
    /// İncelemeye al
    /// </summary>
    public Result StartReview(string assignedTo)
    {
        if (!Status.CanTransitionTo(DisputeStatus.UnderReview))
            return Result.Failure($"Bu durumda incelemeye alınamaz. Mevcut durum: {Status.DisplayName}");

        if (string.IsNullOrWhiteSpace(assignedTo))
            return Result.Failure("Atanan kişi belirtilmeli");

        Status = DisputeStatus.UnderReview;
        AssignedTo = assignedTo;
        AssignedAt = DateTime.UtcNow;
        MarkAsUpdated(assignedTo);

        AddDomainEvent(new DisputeUnderReviewEvent(Id, DisputeNumber, assignedTo));

        return Result.Success();
    }

    /// <summary>
    /// Üye işyerinden bilgi iste
    /// </summary>
    public Result RequestInformation(string requestedInfo, string operatorUsername)
    {
        if (!Status.CanTransitionTo(DisputeStatus.InformationRequested))
            return Result.Failure($"Bu durumda bilgi istenemez. Mevcut durum: {Status.DisplayName}");

        if (string.IsNullOrWhiteSpace(requestedInfo))
            return Result.Failure("İstenen bilgi belirtilmeli");

        Status = DisputeStatus.InformationRequested;
        MarkAsUpdated(operatorUsername);

        AddNote($"Üye işyerinden bilgi istendi: {requestedInfo}", operatorUsername, false);

        AddDomainEvent(new InformationRequestedEvent(Id, DisputeNumber, MerchantId, requestedInfo));

        return Result.Success();
    }

    /// <summary>
    /// Üye işyeri yanıtı kaydet
    /// </summary>
    public Result RecordMerchantResponse(string response, bool accepted, string operatorUsername)
    {
        if (Status != DisputeStatus.InformationRequested && Status != DisputeStatus.UnderReview)
            return Result.Failure("Üye işyeri yanıtı sadece inceleme veya bilgi bekleme durumunda kaydedilebilir");

        if (string.IsNullOrWhiteSpace(response))
            return Result.Failure("Yanıt boş olamaz");

        MerchantResponse = response;
        MerchantResponseDate = DateTime.UtcNow;
        Status = accepted ? DisputeStatus.AcceptedByMerchant : DisputeStatus.RejectedByMerchant;
        MarkAsUpdated(operatorUsername);

        AddNote($"Üye işyeri yanıtı: {response}", operatorUsername, true);

        return Result.Success();
    }

    /// <summary>
    /// Bankaya yönlendir
    /// </summary>
    public Result EscalateToBank(string escalationReason, string operatorUsername)
    {
        if (!Status.CanTransitionTo(DisputeStatus.EscalatedToBank))
            return Result.Failure($"Bu durumda bankaya yönlendirilemez. Mevcut durum: {Status.DisplayName}");

        if (string.IsNullOrWhiteSpace(escalationReason))
            return Result.Failure("Yönlendirme nedeni belirtilmeli");

        Status = DisputeStatus.EscalatedToBank;
        EscalatedAt = DateTime.UtcNow;
        MarkAsUpdated(operatorUsername);

        AddNote($"Bankaya yönlendirildi: {escalationReason}", operatorUsername, true);

        AddDomainEvent(new DisputeEscalatedEvent(Id, DisputeNumber, escalationReason));

        return Result.Success();
    }

    /// <summary>
    /// Müşteri lehine çöz
    /// </summary>
    public Result ResolveInFavorOfCustomer(decimal refundAmount, string resolution, string operatorUsername)
    {
        if (!Status.CanTransitionTo(DisputeStatus.ResolvedInFavorOfCustomer))
            return Result.Failure($"Bu durumda müşteri lehine çözülemez. Mevcut durum: {Status.DisplayName}");

        if (refundAmount <= 0 || refundAmount > DisputedAmount)
            return Result.Failure("Geçersiz iade tutarı");

        if (string.IsNullOrWhiteSpace(resolution))
            return Result.Failure("Çözüm açıklaması belirtilmeli");

        Status = DisputeStatus.ResolvedInFavorOfCustomer;
        RefundAmount = refundAmount;
        Resolution = resolution;
        ResolvedAt = DateTime.UtcNow;
        ResolvedBy = operatorUsername;
        MarkAsUpdated(operatorUsername);

        AddDomainEvent(new DisputeResolvedEvent(Id, DisputeNumber, true, refundAmount, resolution));

        return Result.Success();
    }

    /// <summary>
    /// Üye işyeri lehine çöz
    /// </summary>
    public Result ResolveInFavorOfMerchant(string resolution, string operatorUsername)
    {
        if (!Status.CanTransitionTo(DisputeStatus.ResolvedInFavorOfMerchant))
            return Result.Failure($"Bu durumda üye işyeri lehine çözülemez. Mevcut durum: {Status.DisplayName}");

        if (string.IsNullOrWhiteSpace(resolution))
            return Result.Failure("Çözüm açıklaması belirtilmeli");

        Status = DisputeStatus.ResolvedInFavorOfMerchant;
        Resolution = resolution;
        ResolvedAt = DateTime.UtcNow;
        ResolvedBy = operatorUsername;
        MarkAsUpdated(operatorUsername);

        AddDomainEvent(new DisputeResolvedEvent(Id, DisputeNumber, false, null, resolution));

        return Result.Success();
    }

    /// <summary>
    /// İtirazı iptal et
    /// </summary>
    public Result Cancel(string reason, string operatorUsername)
    {
        if (!Status.CanTransitionTo(DisputeStatus.Cancelled))
            return Result.Failure($"Bu durumda iptal edilemez. Mevcut durum: {Status.DisplayName}");

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure("İptal nedeni belirtilmeli");

        Status = DisputeStatus.Cancelled;
        Resolution = $"İptal edildi: {reason}";
        ResolvedAt = DateTime.UtcNow;
        ResolvedBy = operatorUsername;
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Belge ekle
    /// </summary>
    public Result AddDocument(string fileName, string fileType, string filePath, long fileSize, string uploadedBy, string? description = null)
    {
        if (Status.IsFinal)
            return Result.Failure("Çözülmüş itirazlara belge eklenemez");

        var document = DisputeDocument.Create(Id, fileName, fileType, filePath, fileSize, uploadedBy, description);
        _documents.Add(document);

        return Result.Success();
    }

    /// <summary>
    /// Not ekle
    /// </summary>
    public void AddNote(string note, string createdByUser, bool isInternal = true)
    {
        var disputeNote = DisputeNote.Create(Id, note, createdByUser, isInternal);
        _notes.Add(disputeNote);
    }

    /// <summary>
    /// Müşteri beyanı ekle
    /// </summary>
    public Result AddCustomerStatement(string statement, string operatorUsername)
    {
        if (string.IsNullOrWhiteSpace(statement))
            return Result.Failure("Müşteri beyanı boş olamaz");

        CustomerStatement = statement;
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Süre aşımı kontrolü
    /// </summary>
    public bool IsOverdue => !Status.IsFinal && DateTime.UtcNow > DueDate;

    /// <summary>
    /// Kalan gün
    /// </summary>
    public int RemainingDays => Status.IsFinal ? 0 : Math.Max(0, (DueDate - DateTime.UtcNow).Days);

    private static string GenerateDisputeNumber()
    {
        return $"DSP{DateTime.UtcNow:yyyyMMdd}{new Random().Next(100000, 999999)}";
    }
}