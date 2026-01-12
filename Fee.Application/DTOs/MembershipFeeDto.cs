namespace Fee.Application.DTOs;

/// <summary>
/// Aidat DTO
/// </summary>
public class MembershipFeeDto
{
    public Guid Id { get; set; }
    public string FeeName { get; set; } = null!;
    public string FeeType { get; set; } = null!;
    public string FeeTypeDisplayName { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Period { get; set; } = null!;
    public string PeriodDisplayName { get; set; } = null!;
    public int GracePeriodDays { get; set; }
    public decimal LateFeeRate { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public decimal? MinimumTransactionVolume { get; set; }
    public int? MinimumTransactionCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Aidat oluşturma DTO
/// </summary>
public class CreateMembershipFeeDto
{
    public string FeeName { get; set; } = null!;
    public int FeeTypeId { get; set; }
    public decimal Amount { get; set; }
    public int PeriodId { get; set; }
    public int GracePeriodDays { get; set; } = 30;
    public decimal LateFeeRate { get; set; } = 2.5m;
    public string? Description { get; set; }
    public decimal? MinimumTransactionVolume { get; set; }
    public int? MinimumTransactionCount { get; set; }
}