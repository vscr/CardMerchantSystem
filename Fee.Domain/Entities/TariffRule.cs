using Fee.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace Fee.Domain.Entities;

/// <summary>
/// Tarife Kuralı - Komisyon hesaplama detayları
/// </summary>
public class TariffRule : Entity
{
    public Guid TariffId { get; private set; }
    public CalculationType CalculationType { get; private set; } = null!;
    public decimal Rate { get; private set; }
    public decimal? MinimumFee { get; private set; }
    public decimal? MaximumFee { get; private set; }
    public string? MCC { get; private set; }
    public int? InstallmentCount { get; private set; }
    public decimal? VolumeFrom { get; private set; }
    public decimal? VolumeTo { get; private set; }
    public bool IsActive { get; private set; }
    public int Priority { get; private set; }

    // EF Core için
    private TariffRule() { }

    /// <summary>
    /// Yeni tarife kuralı oluşturur
    /// </summary>
    public static Result<TariffRule> Create(
        Guid tariffId,
        CalculationType calculationType,
        decimal rate,
        decimal? minimumFee = null,
        decimal? maximumFee = null,
        string? mcc = null,
        int? installmentCount = null,
        decimal? volumeFrom = null,
        decimal? volumeTo = null)
    {
        if (rate < 0)
            return Result.Failure<TariffRule>("Oran negatif olamaz");

        if (minimumFee.HasValue && maximumFee.HasValue && minimumFee > maximumFee)
            return Result.Failure<TariffRule>("Minimum ücret maksimumdan büyük olamaz");

        if (volumeFrom.HasValue && volumeTo.HasValue && volumeFrom > volumeTo)
            return Result.Failure<TariffRule>("Hacim başlangıcı bitişten büyük olamaz");

        // Öncelik hesapla (ne kadar spesifik o kadar yüksek)
        var priority = 0;
        if (!string.IsNullOrEmpty(mcc)) priority += 100;
        if (installmentCount.HasValue) priority += 50;
        if (volumeFrom.HasValue || volumeTo.HasValue) priority += 25;

        var rule = new TariffRule
        {
            TariffId = tariffId,
            CalculationType = calculationType,
            Rate = rate,
            MinimumFee = minimumFee,
            MaximumFee = maximumFee,
            MCC = mcc,
            InstallmentCount = installmentCount,
            VolumeFrom = volumeFrom,
            VolumeTo = volumeTo,
            IsActive = true,
            Priority = priority
        };

        return rule;
    }

    /// <summary>
    /// Komisyon hesaplar
    /// </summary>
    public decimal CalculateFee(decimal amount)
    {
        decimal fee;

        if (CalculationType == Enums.CalculationType.FixedAmount)
        {
            fee = Rate;
        }
        else // Percentage based
        {
            fee = amount * Rate / 100;
        }

        // Minimum kontrolü
        if (MinimumFee.HasValue && fee < MinimumFee.Value)
            fee = MinimumFee.Value;

        // Maximum kontrolü
        if (MaximumFee.HasValue && fee > MaximumFee.Value)
            fee = MaximumFee.Value;

        return Math.Round(fee, 2);
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}