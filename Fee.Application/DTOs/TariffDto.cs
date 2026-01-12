namespace Fee.Application.DTOs;

/// <summary>
/// Tarife DTO
/// </summary>
public class TariffDto
{
    public Guid Id { get; set; }
    public string TariffCode { get; set; } = null!;
    public string TariffName { get; set; } = null!;
    public string? Description { get; set; }
    public string FeeType { get; set; } = null!;
    public string FeeTypeDisplayName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TariffRuleDto> Rules { get; set; } = new();
}

/// <summary>
/// Tarife Kuralı DTO
/// </summary>
public class TariffRuleDto
{
    public Guid Id { get; set; }
    public string CalculationType { get; set; } = null!;
    public string CalculationTypeDisplayName { get; set; } = null!;
    public decimal Rate { get; set; }
    public decimal? MinimumFee { get; set; }
    public decimal? MaximumFee { get; set; }
    public string? MCC { get; set; }
    public int? InstallmentCount { get; set; }
    public decimal? VolumeFrom { get; set; }
    public decimal? VolumeTo { get; set; }
    public bool IsActive { get; set; }
    public int Priority { get; set; }
}

/// <summary>
/// Tarife oluşturma DTO
/// </summary>
public class CreateTariffDto
{
    public string TariffCode { get; set; } = null!;
    public string TariffName { get; set; } = null!;
    public string? Description { get; set; }
    public int FeeTypeId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsDefault { get; set; }
}

/// <summary>
/// Tarife Kuralı ekleme DTO
/// </summary>
public class AddTariffRuleDto
{
    public Guid TariffId { get; set; }
    public int CalculationTypeId { get; set; }
    public decimal Rate { get; set; }
    public decimal? MinimumFee { get; set; }
    public decimal? MaximumFee { get; set; }
    public string? MCC { get; set; }
    public int? InstallmentCount { get; set; }
    public decimal? VolumeFrom { get; set; }
    public decimal? VolumeTo { get; set; }
}