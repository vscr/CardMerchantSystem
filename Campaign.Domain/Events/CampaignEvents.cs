using CardMerchantSystem.Shared.Kernel;

namespace Campaign.Domain.Events;

/// <summary>
/// Kampanya oluşturuldu
/// </summary>
public class CampaignCreatedEvent : DomainEvent
{
    public Guid CampaignId { get; }
    public string CampaignCode { get; }
    public string Name { get; }
    public string CampaignType { get; }

    public CampaignCreatedEvent(Guid campaignId, string campaignCode, string name, string campaignType)
    {
        CampaignId = campaignId;
        CampaignCode = campaignCode;
        Name = name;
        CampaignType = campaignType;
    }
}

/// <summary>
/// Kampanya aktif edildi
/// </summary>
public class CampaignActivatedEvent : DomainEvent
{
    public Guid CampaignId { get; }
    public string CampaignCode { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    public CampaignActivatedEvent(Guid campaignId, string campaignCode, DateTime startDate, DateTime endDate)
    {
        CampaignId = campaignId;
        CampaignCode = campaignCode;
        StartDate = startDate;
        EndDate = endDate;
    }
}

/// <summary>
/// Kampanya duraklatıldı
/// </summary>
public class CampaignPausedEvent : DomainEvent
{
    public Guid CampaignId { get; }
    public string CampaignCode { get; }
    public string Reason { get; }

    public CampaignPausedEvent(Guid campaignId, string campaignCode, string reason)
    {
        CampaignId = campaignId;
        CampaignCode = campaignCode;
        Reason = reason;
    }
}

/// <summary>
/// Kampanya tamamlandı
/// </summary>
public class CampaignCompletedEvent : DomainEvent
{
    public Guid CampaignId { get; }
    public string CampaignCode { get; }
    public int TotalUsageCount { get; }
    public decimal TotalDiscountAmount { get; }

    public CampaignCompletedEvent(Guid campaignId, string campaignCode, int totalUsageCount, decimal totalDiscountAmount)
    {
        CampaignId = campaignId;
        CampaignCode = campaignCode;
        TotalUsageCount = totalUsageCount;
        TotalDiscountAmount = totalDiscountAmount;
    }
}

/// <summary>
/// Kampanya kullanıldı
/// </summary>
public class CampaignUsedEvent : DomainEvent
{
    public Guid CampaignId { get; }
    public string CampaignCode { get; }
    public Guid TransactionId { get; }
    public decimal OriginalAmount { get; }
    public decimal DiscountAmount { get; }
    public decimal FinalAmount { get; }

    public CampaignUsedEvent(Guid campaignId, string campaignCode, Guid transactionId,
        decimal originalAmount, decimal discountAmount, decimal finalAmount)
    {
        CampaignId = campaignId;
        CampaignCode = campaignCode;
        TransactionId = transactionId;
        OriginalAmount = originalAmount;
        DiscountAmount = discountAmount;
        FinalAmount = finalAmount;
    }
}