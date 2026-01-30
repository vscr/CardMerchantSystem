using CardMerchantSystem.Shared.Events;
using CardMerchantSystem.Shared.Kernel;
using Merchant.Domain.Enums;
using Merchant.Domain.Events;
using Merchant.Domain.ValueObjects;

namespace Merchant.Domain.Entities;

/// <summary>
/// Üye İşyeri Aggregate Root
/// </summary>
public class MerchantAggregate : AggregateRoot
{
    // İşyeri Bilgileri
    public MerchantCode MerchantCode { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string TradeName { get; private set; } = null!;
    public TaxNumber TaxNumber { get; private set; } = null!;
    public string TaxOffice { get; private set; } = null!;
    public MerchantType MerchantType { get; private set; } = null!;
    public MerchantStatus Status { get; private set; } = null!;

    // İletişim Bilgileri
    public string PhoneNumber { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string District { get; private set; } = null!;

    // Banka Bilgileri
    public IBAN IBAN { get; private set; } = null!;

    // Sözleşme Bilgileri
    public decimal CommissionRate { get; private set; }
    public DateTime? ContractStartDate { get; private set; }
    public DateTime? ContractEndDate { get; private set; }

    // Onay/Red Bilgileri
    public string? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    // Terminaller
    private List<Terminal> _terminals = new();
    public IReadOnlyCollection<Terminal> Terminals => _terminals.AsReadOnly();

    // EF Core için
    private MerchantAggregate() { }

    /// <summary>
    /// Yeni üye işyeri oluşturur
    /// </summary>
    public static Result<MerchantAggregate> Create(
        string name,
        string tradeName,
        TaxNumber taxNumber,
        string taxOffice,
        MerchantType merchantType,
        string phoneNumber,
        string email,
        string address,
        string city,
        string district,
        IBAN iban,
        decimal commissionRate)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<MerchantAggregate>("İşyeri adı boş olamaz");

        if (string.IsNullOrWhiteSpace(tradeName))
            return Result.Failure<MerchantAggregate>("Ticari unvan boş olamaz");

        if (string.IsNullOrWhiteSpace(taxOffice))
            return Result.Failure<MerchantAggregate>("Vergi dairesi boş olamaz");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Result.Failure<MerchantAggregate>("Telefon numarası boş olamaz");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return Result.Failure<MerchantAggregate>("Geçerli bir e-posta adresi giriniz");

        if (commissionRate < 0 || commissionRate > 100)
            return Result.Failure<MerchantAggregate>("Komisyon oranı 0-100 arasında olmalıdır");

        var merchant = new MerchantAggregate
        {
            MerchantCode = MerchantCode.Generate(),
            Name = name.Trim(),
            TradeName = tradeName.Trim(),
            TaxNumber = taxNumber,
            TaxOffice = taxOffice.Trim(),
            MerchantType = merchantType,
            Status = MerchantStatus.Pending,
            PhoneNumber = phoneNumber.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            Address = address.Trim(),
            City = city.Trim(),
            District = district.Trim(),
            IBAN = iban,
            CommissionRate = commissionRate
        };

        merchant.AddDomainEvent(new MerchantCreatedEvent(
            merchant.Id,
            merchant.MerchantCode.Value,
            taxNumber.Value));

        return merchant;
    }

    /// <summary>
    /// İncelemeye alır
    /// </summary>
    public Result StartReview(string reviewerUsername)
    {
        if (!Status.CanTransitionTo(MerchantStatus.UnderReview))
            return Result.Failure($"Bu durumda inceleme başlatılamaz. Mevcut durum: {Status.DisplayName}",
                ErrorCodes.MerchantNotActive);

        Status = MerchantStatus.UnderReview;
        MarkAsUpdated(reviewerUsername);

        return Result.Success();
    }

    /// <summary>
    /// Onaylar
    /// </summary>
    public Result Approve(string approverUsername)
    {
        if (Status == MerchantStatus.Pending)
        {
            var reviewResult = StartReview(approverUsername);
            if (reviewResult.IsFailure)
                return reviewResult;
        }

        if (!Status.CanTransitionTo(MerchantStatus.Approved))
            return Result.Failure($"Bu durumda onaylanamaz. Mevcut durum: {Status.DisplayName}",
                ErrorCodes.MerchantNotActive);

        Status = MerchantStatus.Approved;
        ApprovedBy = approverUsername;
        ApprovedAt = DateTime.UtcNow;
        MarkAsUpdated(approverUsername);

        // Local event
        AddDomainEvent(new MerchantApprovedEvent(Id, MerchantCode.Value));

        // Integration event
        AddDomainEvent(new MerchantApprovedIntegrationEvent(
            Id,
            MerchantCode.Value,
            Name
        ));

        return Result.Success();
    }

    /// <summary>
    /// Reddeder
    /// </summary>
    public Result Reject(string reason, string rejectorUsername)
    {
        if (Status == MerchantStatus.Pending)
        {
            var reviewResult = StartReview(rejectorUsername);
            if (reviewResult.IsFailure)
                return reviewResult;
        }

        if (!Status.CanTransitionTo(MerchantStatus.Rejected))
            return Result.Failure($"Bu durumda reddedilemez. Mevcut durum: {Status.DisplayName}",
                ErrorCodes.MerchantNotActive);

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure("Red nedeni belirtilmeli");

        Status = MerchantStatus.Rejected;
        RejectionReason = reason;
        MarkAsUpdated(rejectorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Aktif eder
    /// </summary>
    public Result Activate(string operatorUsername)
    {
        if (!Status.CanTransitionTo(MerchantStatus.Active))
            return Result.Failure($"Bu durumda aktif edilemez. Mevcut durum: {Status.DisplayName}",
                ErrorCodes.MerchantNotActive);

        Status = MerchantStatus.Active;
        ContractStartDate = DateTime.UtcNow;
        MarkAsUpdated(operatorUsername);

        AddDomainEvent(new MerchantActivatedEvent(Id, MerchantCode.Value));

        return Result.Success();
    }

    /// <summary>
    /// Askıya alır
    /// </summary>
    public Result Suspend(string reason, string operatorUsername)
    {
        if (!Status.CanTransitionTo(MerchantStatus.Suspended))
            return Result.Failure($"Bu durumda askıya alınamaz. Mevcut durum: {Status.DisplayName}",
                ErrorCodes.MerchantNotActive);

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure("Askıya alma nedeni belirtilmeli");

        Status = MerchantStatus.Suspended;
        MarkAsUpdated(operatorUsername);

        AddDomainEvent(new MerchantSuspendedEvent(Id, MerchantCode.Value, reason));

        return Result.Success();
    }

    /// <summary>
    /// Kapatır
    /// </summary>
    public Result Close(string operatorUsername)
    {
        if (!Status.CanTransitionTo(MerchantStatus.Closed))
            return Result.Failure($"Bu durumda kapatılamaz. Mevcut durum: {Status.DisplayName}",
                ErrorCodes.MerchantNotActive);

        Status = MerchantStatus.Closed;
        ContractEndDate = DateTime.UtcNow;
        MarkAsUpdated(operatorUsername);

        // Tüm terminalleri kaldır
        foreach (var terminal in _terminals.Where(t => t.Status != TerminalStatus.Removed))
        {
            terminal.Remove();
        }

        return Result.Success();
    }

    /// <summary>
    /// Terminal ekler
    /// </summary>
    public Result<Terminal> AddTerminal(
        TerminalType terminalType,
        string? serialNumber = null,
        string? model = null,
        string? location = null)
    {
        if (Status != MerchantStatus.Active && Status != MerchantStatus.Approved)
            return Result.Failure<Terminal>("Sadece onaylı veya aktif işyerlerine terminal eklenebilir",
                ErrorCodes.MerchantNotActive);

        var terminalCode = TerminalId.Generate();
        var terminal = new Terminal(
            Id,
            terminalCode,
            terminalType,
            serialNumber,
            model,
            location);

        _terminals.Add(terminal);

        AddDomainEvent(new TerminalAddedEvent(Id, terminal.Id, terminalCode.Value));

        return terminal;
    }

    /// <summary>
    /// Terminali aktif eder
    /// </summary>
    public Result ActivateTerminal(Guid terminalId, string operatorUsername)
    {
        var terminal = _terminals.FirstOrDefault(t => t.Id == terminalId);
        if (terminal == null)
            return Result.Failure("Terminal bulunamadı", ErrorCodes.TerminalNotFound);

        if (Status != MerchantStatus.Active)
            return Result.Failure("Sadece aktif işyerlerinin terminalleri aktif edilebilir",
                ErrorCodes.MerchantNotActive);

        var result = terminal.Activate(operatorUsername);
        if (result.IsFailure)
            return result;

        // Local event
        AddDomainEvent(new TerminalActivatedEvent(terminal.Id, terminal.TerminalCode.Value));

        // Integration event
        AddDomainEvent(new TerminalActivatedIntegrationEvent(
            terminal.Id,
            terminal.TerminalCode.Value,
            Id,
            MerchantCode.Value
        ));

        return Result.Success();
    }

    /// <summary>
    /// Komisyon oranını günceller
    /// </summary>
    public Result UpdateCommissionRate(decimal newRate, string operatorUsername)
    {
        if (newRate < 0 || newRate > 100)
            return Result.Failure("Komisyon oranı 0-100 arasında olmalıdır");

        CommissionRate = newRate;
        MarkAsUpdated(operatorUsername);

        return Result.Success();
    }

    /// <summary>
    /// Aktif terminal sayısı
    /// </summary>
    public int ActiveTerminalCount => _terminals.Count(t => t.Status == TerminalStatus.Active);
}