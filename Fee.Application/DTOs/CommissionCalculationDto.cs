namespace Fee.Application.DTOs;

/// <summary>
/// Komisyon hesaplama request DTO
/// </summary>
public class CalculateCommissionRequestDto
{
    public string MerchantId { get; set; } = null!;
    public decimal TransactionAmount { get; set; }
    public string? MCC { get; set; }
    public int InstallmentCount { get; set; } = 1;
}

/// <summary>
/// Komisyon hesaplama response DTO
/// </summary>
public class CalculateCommissionResponseDto
{
    public decimal TransactionAmount { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal MerchantNetAmount { get; set; }
    public decimal CommissionRate { get; set; }
    public string TariffCode { get; set; } = null!;
    public string CalculationType { get; set; } = null!;

    // Kırılım
    public decimal BankShare { get; set; }
    public decimal InterchangeFee { get; set; }
    public decimal BKMFee { get; set; }

    // Oranlar
    public decimal BankShareRate { get; set; }
    public decimal InterchangeRate { get; set; }
    public decimal BKMRate { get; set; }
}

/// <summary>
/// Komisyon dağılımı DTO
/// </summary>
public class CommissionBreakdownDto
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public string MerchantId { get; set; } = null!;
    public decimal TransactionAmount { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal BankShare { get; set; }
    public decimal InterchangeFee { get; set; }
    public decimal BKMFee { get; set; }
    public decimal MerchantDiscount { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal MerchantNetAmount { get; set; }
    public string? MCC { get; set; }
    public int InstallmentCount { get; set; }
    public DateTime TransactionDate { get; set; }
}

/// <summary>
/// Üye İşyeri komisyon özeti DTO
/// </summary>
public class MerchantCommissionSummaryDto
{
    public string MerchantId { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TransactionCount { get; set; }
    public decimal TotalTransactionAmount { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal TotalBankShare { get; set; }
    public decimal TotalInterchangeFee { get; set; }
    public decimal TotalBKMFee { get; set; }
    public decimal TotalMerchantNet { get; set; }
    public decimal AverageCommissionRate { get; set; }
}