using Campaign.Domain.Enums;
using Campaign.Domain.Events;
using CardMerchantSystem.Shared.Kernel;

namespace Campaign.Domain.Entities;

/// <summary>
/// Kampanya Aggregate Root
/// </summary>
public class CampaignAggregate : AggregateRoot
{
    // Temel Bilgiler
    public string CampaignCode { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public CampaignStatus Status { get; private set; } = null!;
    public CampaignType CampaignType { get; private set; } = null!;
    public DiscountType DiscountType { get; private set; } = null!;
    public TargetAudience TargetAudience { get; private set; } = null!;

    // İndirim/Kazanım Bilgileri
    public decimal DiscountValue { get; private set; }
    public decimal? MaxDiscountAmount { get; private set; }
    public decimal? MinTransactionAmount { get; private set; }
    public int PointsMultiplier { get; private set; }

    // Bütçe ve Limit Bilgileri
    public decimal? TotalBudget { get; private set; }
    public decimal UsedBudget { get; private set; }
    public int? MaxUsageCount { get; private set; }
    public int CurrentUsageCount { get; private set; }
    public int? MaxUsagePerCustomer { get; private set; }

    // Tarih Bilgileri
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    // Üye İşyeri Kısıtlaması
    public bool IsAllMerchants { get; private set; }
    public List<Guid>? AllowedMerchantIds { get; private set; }

    // Onay Bilgileri
    public string? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }

    // Alt Koleksiyonlar
    private readonly List<CampaignRule> _rules = new();
    public IReadOnlyCollection<CampaignRule> Rules => _rules.AsReadOnly();

    private readonly List<CampaignUsage> _usages = new();
    public IReadOnlyCollection<CampaignUsage> Usages => _usages.AsReadOnly();

    // EF Core için
    private CampaignAggregate() { }

    /// <summary>
    /// Yeni kampanya oluşturur
    /// </summary>
    public static Result<CampaignAggregate> Create(
        string name,
        string description,
        CampaignType campaignType,
        DiscountType discountType,
        TargetAudience targetAudience,
        decimal discountValue,
        DateTime startDate,
        DateTime endDate,
        decimal? maxDiscountAmount = null,
        decimal? minTransactionAmount = null,
        decimal? totalBudget = null,
        int? maxUsageCount = null,
        int? maxUsagePerCustomer = null,
        int pointsMultiplier = 1)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<CampaignAggregate>("Kampanya adı boş olamaz");

        if (discountValue <= 0)
            return Result.Failure<CampaignAggregate>("İndirim değeri sıfırdan büyük olmalı");

        if (discountType == DiscountType.Percentage && discountValue > 100)
            return Result.Failure<CampaignAggregate>("Yüzde indirim 100'den büyük olamaz");

        if (startDate >= endDate)
            return Result.Failure<CampaignAggregate>("Başlangıç tarihi bitiş tarihinden önce olmalı");

        if (startDate < DateTime.UtcNow.Date)
            return Result.Failure<CampaignAggregate>("Başlangıç tarihi geçmiş olamaz");

        var campaign = new CampaignAggregate
        {
            CampaignCode = GenerateCampaignCode(),
            Name = name.Trim(),
            Description = description?.Trim() ?? string.Empty,
            Status = CampaignStatus.Draft,
            CampaignType = campaignType,
            DiscountType = discountType,
            TargetAudience = targetAudience,
            DiscountValue = discountValue,
            MaxDiscountAmount = maxDiscountAmount,
            MinTransactionAmount = minTransactionAmount,
            TotalBudget = totalBudget,
            UsedBudget = 0,
            MaxUsageCount = maxUsageCount,
            CurrentUsageCount = 0,
            MaxUsagePerCustomer = maxUsagePerCustomer,
            PointsMultiplier = pointsMultiplier,
            StartDate = startDate,
            EndDate = endDate,
            IsAllMerchants = true
        };

        campaign.AddDomainEvent(new CampaignCreatedEvent(
            campaign.Id,
            campaign.CampaignCode,
            campaign.Name,
            campaignType.DisplayName));

        return campaign;
    }

    /// <summary>
    /// Onaya gönder
    /// </summary>
    public Result SubmitForApproval(string username)
    {
        if (!Status.CanTransitionTo(CampaignStatus.Pending))
            return Result.Failure($"Bu durumda onaya gönderilemez. Mevcut durum: {Status.DisplayName}");

        Status = CampaignStatus.Pending;
        MarkAsUpdated(username);

        return Result.Success();
    }

    /// <summary>
    /// Kampanyayı aktif et
    /// </summary>
    public Result Activate(string approverUsername)
    {
        if (!Status.CanTransitionTo(CampaignStatus.Active))
            return Result.Failure($"Bu durumda aktif edilemez. Mevcut durum: {Status.DisplayName}");

        Status = CampaignStatus.Active;
        ApprovedBy = approverUsername;
        ApprovedAt = DateTime.UtcNow;
        MarkAsUpdated(approverUsername);

        AddDomainEvent(new CampaignActivatedEvent(Id, CampaignCode, StartDate, EndDate));

        return Result.Success();
    }

    /// <summary>
    /// Kampanyayı duraklat
    /// </summary>
    public Result Pause(string reason, string username)
    {
        if (!Status.CanTransitionTo(CampaignStatus.Paused))
            return Result.Failure($"Bu durumda duraklatılamaz. Mevcut durum: {Status.DisplayName}");

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure("Duraklatma nedeni belirtilmeli");

        Status = CampaignStatus.Paused;
        MarkAsUpdated(username);

        AddDomainEvent(new CampaignPausedEvent(Id, CampaignCode, reason));

        return Result.Success();
    }

    /// <summary>
    /// Kampanyayı devam ettir
    /// </summary>
    public Result Resume(string username)
    {
        if (!Status.CanTransitionTo(CampaignStatus.Active))
            return Result.Failure($"Bu durumda devam ettirilemez. Mevcut durum: {Status.DisplayName}");

        if (DateTime.UtcNow > EndDate)
            return Result.Failure("Kampanya süresi dolmuş");

        Status = CampaignStatus.Active;
        MarkAsUpdated(username);

        return Result.Success();
    }

    /// <summary>
    /// Kampanyayı tamamla
    /// </summary>
    public Result Complete(string username)
    {
        if (!Status.CanTransitionTo(CampaignStatus.Completed))
            return Result.Failure($"Bu durumda tamamlanamaz. Mevcut durum: {Status.DisplayName}");

        Status = CampaignStatus.Completed;
        MarkAsUpdated(username);

        AddDomainEvent(new CampaignCompletedEvent(Id, CampaignCode, CurrentUsageCount, UsedBudget));

        return Result.Success();
    }

    /// <summary>
    /// Kampanyayı iptal et
    /// </summary>
    public Result Cancel(string reason, string username)
    {
        if (!Status.CanTransitionTo(CampaignStatus.Cancelled))
            return Result.Failure($"Bu durumda iptal edilemez. Mevcut durum: {Status.DisplayName}");

        Status = CampaignStatus.Cancelled;
        MarkAsUpdated(username);

        return Result.Success();
    }

    /// <summary>
    /// Kural ekle
    /// </summary>
    public Result AddRule(string ruleName, string ruleType, string operatorType, string value)
    {
        if (Status.IsFinal)
            return Result.Failure("Tamamlanmış veya iptal edilmiş kampanyaya kural eklenemez");

        var rule = CampaignRule.Create(Id, ruleName, ruleType, operatorType, value);
        _rules.Add(rule);

        return Result.Success();
    }

    /// <summary>
    /// Kampanyayı kullan
    /// </summary>
    public Result<CampaignUsage> Use(
        Guid transactionId,
        string cardNumberMasked,
        Guid merchantId,
        string merchantCode,
        decimal transactionAmount,
        string? mcc = null,
        string? cardBin = null)
    {
        // Durum kontrolü
        if (!Status.IsUsable)
            return Result.Failure<CampaignUsage>($"Kampanya kullanılamaz. Durum: {Status.DisplayName}");

        // Tarih kontrolü
        var now = DateTime.UtcNow;
        if (now < StartDate || now > EndDate)
            return Result.Failure<CampaignUsage>("Kampanya tarihi geçerli değil");

        // Kullanım limiti kontrolü
        if (MaxUsageCount.HasValue && CurrentUsageCount >= MaxUsageCount.Value)
            return Result.Failure<CampaignUsage>("Kampanya kullanım limiti doldu");

        // Bütçe kontrolü
        var discountAmount = CalculateDiscount(transactionAmount);
        if (TotalBudget.HasValue && (UsedBudget + discountAmount) > TotalBudget.Value)
            return Result.Failure<CampaignUsage>("Kampanya bütçesi yetersiz");

        // Minimum tutar kontrolü
        if (MinTransactionAmount.HasValue && transactionAmount < MinTransactionAmount.Value)
            return Result.Failure<CampaignUsage>($"Minimum işlem tutarı {MinTransactionAmount.Value} TL olmalı");

        // Üye işyeri kontrolü
        if (!IsAllMerchants && AllowedMerchantIds != null && !AllowedMerchantIds.Contains(merchantId))
            return Result.Failure<CampaignUsage>("Bu üye işyeri kampanyaya dahil değil");

        // Kural kontrolü
        foreach (var rule in _rules.Where(r => r.IsActive))
        {
            if (!rule.Evaluate(transactionAmount, mcc, cardBin))
                return Result.Failure<CampaignUsage>($"Kampanya kuralı sağlanmadı: {rule.RuleName}");
        }

        // İndirim hesapla
        var finalAmount = transactionAmount - discountAmount;
        var pointsEarned = CampaignType.IsPointsBased ? (int)(transactionAmount * PointsMultiplier) : 0;

        // Kullanım kaydı oluştur
        var usage = CampaignUsage.Create(
            Id,
            transactionId,
            cardNumberMasked,
            merchantId,
            merchantCode,
            transactionAmount,
            discountAmount,
            finalAmount,
            pointsEarned);

        _usages.Add(usage);
        CurrentUsageCount++;
        UsedBudget += discountAmount;

        AddDomainEvent(new CampaignUsedEvent(
            Id, CampaignCode, transactionId, transactionAmount, discountAmount, finalAmount));

        return usage;
    }

    /// <summary>
    /// İndirim hesapla
    /// </summary>
    public decimal CalculateDiscount(decimal transactionAmount)
    {
        decimal discount;

        if (DiscountType == Enums.DiscountType.Percentage)
        {
            discount = transactionAmount * (DiscountValue / 100);
        }
        else
        {
            discount = DiscountValue;
        }

        // Maksimum indirim kontrolü
        if (MaxDiscountAmount.HasValue && discount > MaxDiscountAmount.Value)
        {
            discount = MaxDiscountAmount.Value;
        }

        // İndirim işlem tutarından büyük olamaz
        if (discount > transactionAmount)
        {
            discount = transactionAmount;
        }

        return Math.Round(discount, 2);
    }

    /// <summary>
    /// Üye işyeri listesini ayarla
    /// </summary>
    public Result SetAllowedMerchants(List<Guid> merchantIds, string username)
    {
        if (Status.IsFinal)
            return Result.Failure("Tamamlanmış veya iptal edilmiş kampanya değiştirilemez");

        IsAllMerchants = merchantIds == null || merchantIds.Count == 0;
        AllowedMerchantIds = IsAllMerchants ? null : merchantIds;
        MarkAsUpdated(username);

        return Result.Success();
    }

    /// <summary>
    /// Kalan bütçe
    /// </summary>
    public decimal? RemainingBudget => TotalBudget.HasValue ? TotalBudget.Value - UsedBudget : null;

    /// <summary>
    /// Kalan kullanım hakkı
    /// </summary>
    public int? RemainingUsageCount => MaxUsageCount.HasValue ? MaxUsageCount.Value - CurrentUsageCount : null;

    /// <summary>
    /// Kampanya aktif mi?
    /// </summary>
    public bool IsCurrentlyActive => Status.IsUsable && DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;

    private static string GenerateCampaignCode()
    {
        return $"CMP{DateTime.UtcNow:yyyyMMdd}{new Random().Next(10000, 99999)}";
    }
}