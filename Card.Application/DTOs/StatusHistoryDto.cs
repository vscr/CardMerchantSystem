namespace Card.Application.DTOs;

/// <summary>
/// Başvuru durum geçmişi DTO
/// </summary>
public class StatusHistoryDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = null!;
    public string StatusDisplayName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string ChangedBy { get; set; } = null!;
    public DateTime ChangedAt { get; set; }
}