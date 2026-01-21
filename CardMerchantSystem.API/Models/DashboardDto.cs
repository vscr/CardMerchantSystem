namespace CardMerchantSystem.API.Models;

public class DashboardDto
{
    public CardStats Cards { get; set; } = new();
    public TransactionStats Transactions { get; set; } = new();
    public BlockStats Blocks { get; set; } = new();
    public WorkOrderStats WorkOrders { get; set; } = new();
    public MerchantStats Merchants { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class CardStats
{
    public int TotalCards { get; set; }
    public int ActiveCards { get; set; }
    public int BlockedCards { get; set; }
    public int TodayApplications { get; set; }
    public int PendingApplications { get; set; }
}

public class TransactionStats
{
    public int TodayCount { get; set; }
    public decimal TodayAmount { get; set; }
    public int TodaySuccessful { get; set; }
    public int TodayFailed { get; set; }
    public decimal TodaySuccessRate { get; set; }
}

public class BlockStats
{
    public int ActiveBlocks { get; set; }
    public int PendingVerification { get; set; }
    public int TodayNewBlocks { get; set; }
    public int TodayResolved { get; set; }
}

public class WorkOrderStats
{
    public int OpenOrders { get; set; }
    public int OverdueOrders { get; set; }
    public int TodayCreated { get; set; }
    public int TodayCompleted { get; set; }
}

public class MerchantStats
{
    public int TotalMerchants { get; set; }
    public int ActiveMerchants { get; set; }
    public int TotalTerminals { get; set; }
    public int ActiveTerminals { get; set; }
}