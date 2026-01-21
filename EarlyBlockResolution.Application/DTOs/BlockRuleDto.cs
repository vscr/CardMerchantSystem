namespace EarlyBlockResolution.Application.DTOs;

public class BlockRuleDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string TriggerReason { get; set; } = null!;
    public string TriggerReasonDisplayName { get; set; } = null!;
    public string Severity { get; set; } = null!;
    public string SeverityDisplayName { get; set; } = null!;
    public decimal? AmountThreshold { get; set; }
    public int? CountThreshold { get; set; }
    public int? TimeWindowMinutes { get; set; }
    public int? FraudScoreThreshold { get; set; }
    public bool AutoBlockEnabled { get; set; }
    public int BlockDurationMinutes { get; set; }
    public bool IsActive { get; set; }
    public int Priority { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateBlockRuleDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int TriggerReasonId { get; set; }
    public int SeverityId { get; set; }
    public int Priority { get; set; }
    public decimal? AmountThreshold { get; set; }
    public int? CountThreshold { get; set; }
    public int? TimeWindowMinutes { get; set; }
    public int? FraudScoreThreshold { get; set; }
    public bool AutoBlockEnabled { get; set; }
    public int BlockDurationMinutes { get; set; }
}