namespace Accounting.Application.DTOs;

/// <summary>
/// Muhasebe Fişi Satırı DTO
/// </summary>
public class JournalEntryLineDto
{
    public Guid Id { get; set; }
    public int LineNumber { get; set; }
    public Guid AccountId { get; set; }
    public string AccountCode { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public decimal NetAmount { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Muhasebe Fişi Satırı Oluşturma DTO
/// </summary>
public class CreateJournalEntryLineDto
{
    public Guid AccountId { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Satır Ekleme DTO
/// </summary>
public class AddJournalEntryLineDto
{
    public Guid JournalEntryId { get; set; }
    public Guid AccountId { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string? Description { get; set; }
}