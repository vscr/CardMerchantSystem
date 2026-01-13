namespace MerchantReport.Application.DTOs;

/// <summary>
/// Üye İşyeri Ekstresi DTO
/// </summary>
public class MerchantStatementDto
{
    public Guid Id { get; set; }
    public string StatementNumber { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string MerchantName { get; set; } = null!;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime StatementDate { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalRefunds { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal TotalSettlement { get; set; }
    public decimal ClosingBalance { get; set; }
    public int SalesCount { get; set; }
    public int RefundCount { get; set; }
    public int ChargebackCount { get; set; }
    public List<MerchantStatementItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Üye İşyeri Ekstresi Özet DTO
/// </summary>
public class MerchantStatementSummaryDto
{
    public Guid Id { get; set; }
    public string StatementNumber { get; set; } = null!;
    public string MerchantName { get; set; } = null!;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal ClosingBalance { get; set; }
    public int SalesCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Üye İşyeri Ekstresi Oluşturma DTO
/// </summary>
public class CreateMerchantStatementDto
{
    public string MerchantId { get; set; } = null!;
    public string MerchantName { get; set; } = null!;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal OpeningBalance { get; set; } = 0;
}