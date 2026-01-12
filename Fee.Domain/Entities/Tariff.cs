using Fee.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace Fee.Domain.Entities;

/// <summary>
/// Tarife - Ana komisyon/ücret tanımı
/// </summary>
public class Tariff : AggregateRoot
{
    public string TariffCode { get; private set; } = null!;
    public string TariffName { get; private set; } = null!;
    public string? Description { get; private set; }
    public FeeType FeeType { get; private set; } = null!;
    public TariffStatus Status { get; private set; } = null!;
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    public bool IsDefault { get; private set; }

    // İlişkili kurallar
    private readonly List<TariffRule> _rules = new();
    public IReadOnlyCollection<TariffRule> Rules => _rules.AsReadOnly();

    // EF Core için
    private Tariff() { }

    /// <summary>
    /// Yeni tarife oluşturur
    /// </summary>
    public static Result<Tariff> Create(
        string tariffCode,
        string tariffName,
        FeeType feeType,
        DateTime effectiveFrom,
        DateTime? effectiveTo = null,
        string? description = null,
        bool isDefault = false)
    {
        if (string.IsNullOrWhiteSpace(tariffCode))
            return Result.Failure<Tariff>("Tarife kodu boş olamaz");

        if (string.IsNullOrWhiteSpace(tariffName))
            return Result.Failure<Tariff>("Tarife adı boş olamaz");

        if (effectiveTo.HasValue && effectiveTo.Value <= effectiveFrom)
            return Result.Failure<Tariff>("Bitiş tarihi başlangıç tarihinden büyük olmalı");

        var tariff = new Tariff
        {
            TariffCode = tariffCode,
            TariffName = tariffName,
            FeeType = feeType,
            Status = TariffStatus.Draft,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo,
            Description = description,
            IsDefault = isDefault
        };

        return tariff;
    }

    /// <summary>
    /// Tarife kuralı ekler
    /// </summary>
    public Result<TariffRule> AddRule(
        CalculationType calculationType,
        decimal rate,
        decimal? minimumFee = null,
        decimal? maximumFee = null,
        string? mcc = null,
        int? installmentCount = null,
        decimal? volumeFrom = null,
        decimal? volumeTo = null)
    {
        var rule = TariffRule.Create(
            Id,
            calculationType,
            rate,
            minimumFee,
            maximumFee,
            mcc,
            installmentCount,
            volumeFrom,
            volumeTo);

        if (rule.IsFailure)
            return Result.Failure<TariffRule>(rule.Error!);

        _rules.Add(rule.Value!);
        return rule.Value!;
    }

    /// <summary>
    /// Tarifeyi aktif eder
    /// </summary>
    public Result Activate()
    {
        if (Status == TariffStatus.Active)
            return Result.Failure("Tarife zaten aktif");

        if (!_rules.Any())
            return Result.Failure("En az bir kural eklenmeli");

        Status = TariffStatus.Active;
        return Result.Success();
    }

    /// <summary>
    /// Tarifeyi askıya alır
    /// </summary>
    public Result Suspend()
    {
        if (Status != TariffStatus.Active)
            return Result.Failure("Sadece aktif tarifeler askıya alınabilir");

        Status = TariffStatus.Suspended;
        return Result.Success();
    }

    /// <summary>
    /// Tarifeyi süresini doldurmuş olarak işaretler
    /// </summary>
    public void MarkAsExpired()
    {
        Status = TariffStatus.Expired;
    }

    /// <summary>
    /// Tarife geçerli mi?
    /// </summary>
    public bool IsValidAt(DateTime date)
    {
        return Status.IsUsable &&
               date >= EffectiveFrom &&
               (!EffectiveTo.HasValue || date <= EffectiveTo.Value);
    }

    /// <summary>
    /// İşlem için uygun kuralı bulur ve komisyon hesaplar
    /// </summary>
    public Result<FeeCalculationResult> CalculateFee(decimal transactionAmount, string? mcc = null, int installmentCount = 1)
    {
        if (!Status.IsUsable)
            return Result.Failure<FeeCalculationResult>("Tarife aktif değil");

        // Uygun kuralı bul (en spesifik olan öncelikli)
        var rule = FindMatchingRule(mcc, installmentCount, transactionAmount);

        if (rule == null)
            return Result.Failure<FeeCalculationResult>("Uygun tarife kuralı bulunamadı");

        var feeAmount = rule.CalculateFee(transactionAmount);

        return new FeeCalculationResult
        {
            TariffId = Id,
            TariffCode = TariffCode,
            RuleId = rule.Id,
            TransactionAmount = transactionAmount,
            FeeAmount = feeAmount,
            Rate = rule.Rate,
            CalculationType = rule.CalculationType.Name,
            NetAmount = transactionAmount - feeAmount
        };
    }

    private TariffRule? FindMatchingRule(string? mcc, int installmentCount, decimal amount)
    {
        // Öncelik: MCC + Taksit + Hacim > MCC + Taksit > MCC > Taksit > Hacim > Genel
        return _rules
            .Where(r => r.IsActive)
            .Where(r => string.IsNullOrEmpty(r.MCC) || r.MCC == mcc)
            .Where(r => !r.InstallmentCount.HasValue || r.InstallmentCount == installmentCount)
            .Where(r => !r.VolumeFrom.HasValue || amount >= r.VolumeFrom)
            .Where(r => !r.VolumeTo.HasValue || amount <= r.VolumeTo)
            .OrderByDescending(r => !string.IsNullOrEmpty(r.MCC) ? 1 : 0)
            .ThenByDescending(r => r.InstallmentCount.HasValue ? 1 : 0)
            .ThenByDescending(r => r.VolumeFrom.HasValue ? 1 : 0)
            .FirstOrDefault();
    }
}

/// <summary>
/// Komisyon hesaplama sonucu
/// </summary>
public class FeeCalculationResult
{
    public Guid TariffId { get; set; }
    public string TariffCode { get; set; } = null!;
    public Guid RuleId { get; set; }
    public decimal TransactionAmount { get; set; }
    public decimal FeeAmount { get; set; }
    public decimal Rate { get; set; }
    public string CalculationType { get; set; } = null!;
    public decimal NetAmount { get; set; }
}