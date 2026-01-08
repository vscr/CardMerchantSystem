namespace Merchant.Application.DTOs;

/// <summary>
/// Terminal response DTO
/// </summary>
public class TerminalDto
{
    public Guid Id { get; set; }
    public Guid MerchantId { get; set; }
    public string TerminalCode { get; set; } = null!;
    public string TerminalType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public string? SerialNumber { get; set; }
    public string? Model { get; set; }
    public string? Location { get; set; }
    public DateTime? InstalledAt { get; set; }
    public string? InstalledBy { get; set; }
    public DateTime CreatedAt { get; set; }
}