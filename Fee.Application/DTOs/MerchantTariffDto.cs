namespace Fee.Application.DTOs;

/// <summary>
/// Üye İşyeri Tarife DTO
/// </summary>
public class MerchantTariffDto
{
    public Guid Id { get; set; }
    public string MerchantId { get; set; } = null!;
    public Guid TariffId { get; set; }
    public string TariffCode { get; set; } = null!;
    public string TariffName { get; set; } = null!;
    public string FeeType { get; set; } = null!;
    public string FeeTypeDisplayName { get; set; } = null!;
    public DateTime AssignedDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public decimal? SpecialRate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Üye İşyeri Tarife atama DTO
/// </summary>
public class AssignMerchantTariffDto
{
    public string MerchantId { get; set; } = null!;
    public Guid TariffId { get; set; }
    public int FeeTypeId { get; set; }
    public decimal? SpecialRate { get; set; }
    public string? Notes { get; set; }
}