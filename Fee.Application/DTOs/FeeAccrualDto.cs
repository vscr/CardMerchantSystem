namespace Fee.Application.DTOs;

/// <summary>
/// Tahakkuk DTO
/// </summary>
public class FeeAccrualDto
{
    public Guid Id { get; set; }
    public string AccrualNumber { get; set; } = null!;
    public string FeeType { get; set; } = null!;
    public string FeeTypeDisplayName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public string Period { get; set; } = null!;
    public string PeriodDisplayName { get; set; } = null!;

    // Kime ait
    public string? MerchantId { get; set; }
    public string? CardNumber { get; set; }
    public string? TerminalId { get; set; }

    // Tutarlar
    public decimal GrossAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }

    // Tarihler
    public DateTime AccrualDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public string AccrualPeriodStart { get; set; } = null!;
    public string AccrualPeriodEnd { get; set; } = null!;
}

/// <summary>
/// Tahakkuk oluşturma DTO
/// </summary>
public class CreateFeeAccrualDto
{
    public int FeeTypeId { get; set; }
    public int PeriodId { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTime DueDate { get; set; }
    public string PeriodStart { get; set; } = null!;
    public string PeriodEnd { get; set; } = null!;
    public string? MerchantId { get; set; }
    public string? CardNumber { get; set; }
    public string? TerminalId { get; set; }
}

/// <summary>
/// Ödeme kaydetme DTO
/// </summary>
public class RecordPaymentDto
{
    public Guid AccrualId { get; set; }
    public decimal Amount { get; set; }
    public string? PaymentReference { get; set; }
}