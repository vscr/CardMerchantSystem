namespace Statement.Application.DTOs;

/// <summary>
/// Kart Ekstre DTO
/// </summary>
public class CardStatementDto
{
    public Guid Id { get; set; }
    public string StatementNumber { get; set; } = null!;
    public string MaskedCardNumber { get; set; } = null!;
    public string CustomerName { get; set; } = null!;

    // Dönem
    public DateTime PeriodStartDate { get; set; }
    public DateTime PeriodEndDate { get; set; }
    public DateTime StatementDate { get; set; }
    public DateTime DueDate { get; set; }

    // Tutarlar
    public decimal PreviousBalance { get; set; }
    public decimal TotalDebits { get; set; }
    public decimal TotalCredits { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal MinimumPayment { get; set; }
    public decimal AvailableCredit { get; set; }
    public decimal CreditLimit { get; set; }

    // Faiz
    public decimal InterestRate { get; set; }
    public decimal InterestAmount { get; set; }

    // Durum
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public string PaymentStatus { get; set; } = null!;
    public string PaymentStatusDisplayName { get; set; } = null!;
    public decimal PaidAmount { get; set; }
    public decimal RemainingBalance { get; set; }
    public DateTime? LastPaymentDate { get; set; }

    // PDF
    public string? PdfPath { get; set; }
    public DateTime? PdfGeneratedAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public List<StatementItemDto> Items { get; set; } = new();
}

/// <summary>
/// Ekstre Özet DTO
/// </summary>
public class CardStatementSummaryDto
{
    public Guid Id { get; set; }
    public string StatementNumber { get; set; } = null!;
    public string MaskedCardNumber { get; set; } = null!;
    public DateTime PeriodEndDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal MinimumPayment { get; set; }
    public decimal RemainingBalance { get; set; }
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
}