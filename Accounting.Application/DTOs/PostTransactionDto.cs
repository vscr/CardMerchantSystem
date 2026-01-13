namespace Accounting.Application.DTOs;

/// <summary>
/// Kart Alışverişi Muhasebeleştirme DTO
/// </summary>
public class PostCardPurchaseDto
{
    public Guid TransactionId { get; set; }
    public string CardNumber { get; set; } = null!;
    public decimal Amount { get; set; }
    public string MerchantName { get; set; } = null!;
}

/// <summary>
/// Kart İadesi Muhasebeleştirme DTO
/// </summary>
public class PostCardRefundDto
{
    public Guid TransactionId { get; set; }
    public string CardNumber { get; set; } = null!;
    public decimal Amount { get; set; }
    public string MerchantName { get; set; } = null!;
}

/// <summary>
/// Kart Ödemesi Muhasebeleştirme DTO
/// </summary>
public class PostCardPaymentDto
{
    public Guid StatementId { get; set; }
    public string CardNumber { get; set; } = null!;
    public decimal Amount { get; set; }
}

/// <summary>
/// Faiz Tahakkuku Muhasebeleştirme DTO
/// </summary>
public class PostInterestAccrualDto
{
    public Guid StatementId { get; set; }
    public string CardNumber { get; set; } = null!;
    public decimal Amount { get; set; }
}

/// <summary>
/// Komisyon Geliri Muhasebeleştirme DTO
/// </summary>
public class PostCommissionIncomeDto
{
    public Guid TransactionId { get; set; }
    public string MerchantId { get; set; } = null!;
    public decimal TotalCommission { get; set; }
    public decimal BankShare { get; set; }
    public decimal InterchangeFee { get; set; }
}

/// <summary>
/// Üye İşyeri Hakediş Muhasebeleştirme DTO
/// </summary>
public class PostMerchantSettlementDto
{
    public string MerchantId { get; set; } = null!;
    public decimal GrossAmount { get; set; }
    public decimal Commission { get; set; }
    public decimal NetAmount { get; set; }
}