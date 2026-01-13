using CardMerchantSystem.Shared.Kernel;

namespace MerchantReport.Domain.Entities;

/// <summary>
/// Üye İşyeri Ekstre Kalemi
/// </summary>
public class MerchantStatementItem : Entity
{
    public Guid StatementId { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public string TransactionType { get; private set; } = null!;
    public string? TransactionId { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public string? CardNumber { get; private set; } // Maskeli
    public string? TerminalId { get; private set; }
    public decimal GrossAmount { get; private set; }
    public decimal CommissionRate { get; private set; }
    public decimal CommissionAmount { get; private set; }
    public decimal NetAmount { get; private set; }
    public int InstallmentCount { get; private set; }
    public string? Description { get; private set; }

    // EF Core için
    private MerchantStatementItem() { }

    /// <summary>
    /// Yeni ekstre kalemi oluşturur
    /// </summary>
    public static MerchantStatementItem Create(
        Guid statementId,
        DateTime transactionDate,
        string transactionType,
        decimal grossAmount,
        decimal commissionRate,
        decimal commissionAmount,
        decimal netAmount,
        string? transactionId = null,
        string? referenceNumber = null,
        string? cardNumber = null,
        string? terminalId = null,
        int installmentCount = 1,
        string? description = null)
    {
        return new MerchantStatementItem
        {
            StatementId = statementId,
            TransactionDate = transactionDate,
            TransactionType = transactionType,
            TransactionId = transactionId,
            ReferenceNumber = referenceNumber,
            CardNumber = cardNumber,
            TerminalId = terminalId,
            GrossAmount = grossAmount,
            CommissionRate = commissionRate,
            CommissionAmount = commissionAmount,
            NetAmount = netAmount,
            InstallmentCount = installmentCount,
            Description = description
        };
    }
}