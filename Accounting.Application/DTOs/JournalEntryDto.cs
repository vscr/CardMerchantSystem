namespace Accounting.Application.DTOs;

/// <summary>
/// Muhasebe Fişi DTO
/// </summary>
public class JournalEntryDto
{
    public Guid Id { get; set; }
    public string EntryNumber { get; set; } = null!;
    public DateTime EntryDate { get; set; }
    public string PeriodCode { get; set; } = null!;
    public string TransactionType { get; set; } = null!;
    public string TransactionTypeDisplayName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? ReferenceNumber { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public bool IsBalanced { get; set; }
    public DateTime? PostedAt { get; set; }
    public string? PostedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<JournalEntryLineDto> Lines { get; set; } = new();
}

/// <summary>
/// Muhasebe Fişi Özet DTO
/// </summary>
public class JournalEntrySummaryDto
{
    public Guid Id { get; set; }
    public string EntryNumber { get; set; } = null!;
    public DateTime EntryDate { get; set; }
    public string TransactionTypeDisplayName { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
}

/// <summary>
/// Muhasebe Fişi Oluşturma DTO
/// </summary>
public class CreateJournalEntryDto
{
    public DateTime EntryDate { get; set; }
    public int TransactionTypeId { get; set; }
    public string Description { get; set; } = null!;
    public string? ReferenceNumber { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public List<CreateJournalEntryLineDto> Lines { get; set; } = new();
}