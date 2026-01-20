namespace EarlyBlockResolution.Application.DTOs;

public class FraudAlertDto
{
    public Guid Id { get; set; }
    public string AlertNumber { get; set; } = null!;
    public Guid CardId { get; set; }
    public string CardNumberMasked { get; set; } = null!;
    public Guid? TransactionId { get; set; }
    public decimal? TransactionAmount { get; set; }
    public string? MerchantName { get; set; }
    public string Reason { get; set; } = null!;
    public string ReasonDisplayName { get; set; } = null!;
    public string Severity { get; set; } = null!;
    public string SeverityDisplayName { get; set; } = null!;
    public int FraudScore { get; set; }
    public Guid? BlockRuleId { get; set; }
    public bool IsProcessed { get; set; }
    public bool BlockCreated { get; set; }
    public Guid? CardBlockId { get; set; }
    public DateTime DetectedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateFraudAlertDto
{
    public Guid CardId { get; set; }
    public string CardNumberMasked { get; set; } = null!;
    public int ReasonId { get; set; }
    public int SeverityId { get; set; }
    public int FraudScore { get; set; }
    public Guid? TransactionId { get; set; }
    public decimal? TransactionAmount { get; set; }
    public string? MerchantName { get; set; }
    public Guid? BlockRuleId { get; set; }
}