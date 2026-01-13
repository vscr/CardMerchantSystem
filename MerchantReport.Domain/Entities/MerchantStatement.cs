using CardMerchantSystem.Shared.Kernel;

namespace MerchantReport.Domain.Entities;

/// <summary>
/// Üye İşyeri Ekstresi
/// </summary>
public class MerchantStatement : AggregateRoot
{
    public string StatementNumber { get; private set; } = null!;
    public string MerchantId { get; private set; } = null!;
    public string MerchantName { get; private set; } = null!;

    // Dönem
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }
    public DateTime StatementDate { get; private set; }

    // Özet
    public decimal OpeningBalance { get; private set; }
    public decimal TotalSales { get; private set; }
    public decimal TotalRefunds { get; private set; }
    public decimal TotalCommission { get; private set; }
    public decimal TotalSettlement { get; private set; }
    public decimal ClosingBalance { get; private set; }

    // İşlem Sayıları
    public int SalesCount { get; private set; }
    public int RefundCount { get; private set; }
    public int ChargebackCount { get; private set; }

    // İlişkili kalemler
    private readonly List<MerchantStatementItem> _items = new();
    public IReadOnlyCollection<MerchantStatementItem> Items => _items.AsReadOnly();

    // EF Core için
    private MerchantStatement() { }

    /// <summary>
    /// Yeni ekstre oluşturur
    /// </summary>
    public static Result<MerchantStatement> Create(
        string merchantId,
        string merchantName,
        DateTime periodStart,
        DateTime periodEnd,
        decimal openingBalance = 0)
    {
        if (string.IsNullOrWhiteSpace(merchantId))
            return Result.Failure<MerchantStatement>("Üye işyeri ID boş olamaz");

        var statement = new MerchantStatement
        {
            StatementNumber = GenerateStatementNumber(),
            MerchantId = merchantId,
            MerchantName = merchantName,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            StatementDate = DateTime.UtcNow,
            OpeningBalance = openingBalance,
            TotalSales = 0,
            TotalRefunds = 0,
            TotalCommission = 0,
            TotalSettlement = 0,
            ClosingBalance = openingBalance,
            SalesCount = 0,
            RefundCount = 0,
            ChargebackCount = 0
        };

        return statement;
    }

    /// <summary>
    /// Kalem ekler
    /// </summary>
    public void AddItem(MerchantStatementItem item)
    {
        _items.Add(item);
        RecalculateTotals();
    }

    /// <summary>
    /// Toplamları yeniden hesaplar
    /// </summary>
    private void RecalculateTotals()
    {
        TotalSales = _items.Where(x => x.TransactionType == "Sale").Sum(x => x.GrossAmount);
        TotalRefunds = _items.Where(x => x.TransactionType == "Refund").Sum(x => x.GrossAmount);
        TotalCommission = _items.Sum(x => x.CommissionAmount);
        TotalSettlement = _items.Sum(x => x.NetAmount);

        SalesCount = _items.Count(x => x.TransactionType == "Sale");
        RefundCount = _items.Count(x => x.TransactionType == "Refund");
        ChargebackCount = _items.Count(x => x.TransactionType == "Chargeback");

        ClosingBalance = OpeningBalance + TotalSales - TotalRefunds - TotalCommission;
    }

    private static string GenerateStatementNumber()
    {
        return $"MSTM{DateTime.UtcNow:yyyyMMdd}{new Random().Next(100000, 999999)}";
    }
}