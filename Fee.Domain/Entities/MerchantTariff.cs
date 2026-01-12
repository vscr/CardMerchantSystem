using Fee.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace Fee.Domain.Entities;

/// <summary>
/// Üye İşyeri - Tarife İlişkisi
/// </summary>
public class MerchantTariff : Entity
{
    public string MerchantId { get; private set; } = null!;
    public Guid TariffId { get; private set; }
    public FeeType FeeType { get; private set; } = null!;
    public DateTime AssignedDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsActive { get; private set; }
    public decimal? SpecialRate { get; private set; }
    public string? Notes { get; private set; }

    // EF Core için
    private MerchantTariff() { }

    /// <summary>
    /// Üye işyerine tarife atar
    /// </summary>
    public static Result<MerchantTariff> Create(
        string merchantId,
        Guid tariffId,
        FeeType feeType,
        decimal? specialRate = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(merchantId))
            return Result.Failure<MerchantTariff>("Üye işyeri ID boş olamaz");

        var merchantTariff = new MerchantTariff
        {
            MerchantId = merchantId,
            TariffId = tariffId,
            FeeType = feeType,
            AssignedDate = DateTime.UtcNow,
            IsActive = true,
            SpecialRate = specialRate,
            Notes = notes
        };

        return merchantTariff;
    }

    /// <summary>
    /// Tarife atamasını sonlandırır
    /// </summary>
    public void Terminate()
    {
        IsActive = false;
        EndDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Özel oran günceller
    /// </summary>
    public void UpdateSpecialRate(decimal? rate)
    {
        SpecialRate = rate;
    }
}